var EmployeeList = [];
var dataGrid;

// Load employee list from server
function loadEmployee(onSuccess) {
    $.ajax({
        url: '/EmpView/GetAll',
        type: 'GET',
        success: function (res) {
            EmployeeList = res;

            // Call onSuccess callback if provided
            if (typeof onSuccess === 'function') {
                onSuccess();
            }

            // Refresh grid if it exists
            if (dataGrid) {
                dataGrid.option("dataSource", EmployeeList);
                dataGrid.refresh();
            } else {
                console.warn("DataGrid is not initialized yet");
            }
        },
        error: function (err) {
            alertify.error("Error loading employees");
            console.error("Load employee error:", err);
        }
    });
}

// Handle edit employee
function editEmployee(employeeId) {
    $.ajax({
        url: `EmpView/GetById/${employeeId}`,
        type: 'GET',
        success: function (response) {
            console.log("Edit employee data:", response);
            // Use the centralized form initialization with edit mode
            initializeEmployeeForm('edit', response);
        },
        error: function (xhr) {
            alertify.error("Error loading employee details");
            console.error("Edit employee error:", xhr.responseText);
        }
    });
}

// Handle delete employee
function deleteEmployee(employeeId) {
    alertify.confirm(
        "Delete Employee",
        "Are you sure you want to delete this employee?",
        function () {
            $.ajax({
                url: `EmpView/DeleteEmp/${employeeId}`,
                type: 'DELETE',
                success: function () {
                    alertify.success("Employee deleted successfully");
                    loadEmployee(function () {
                        if (dataGrid) {
                            dataGrid.option("dataSource", EmployeeList);
                            dataGrid.refresh();
                        }
                    });
                },
                error: function (xhr) {
                    alertify.error("Failed to delete employee");
                    console.error("Delete employee error:", xhr.responseText);
                }
            });
        },
        function () {
            alertify.message('Delete cancelled');
        }
    );
}

// Initialize datagrid
function datagridinit() {
    dataGrid = $("#dataGrid").dxDataGrid({
        dataSource: EmployeeList,
        keyExpr: "employeeID",
        columnFixing: { enabled: true },
        showBorders: true,
        width: null,
        rowAlternationEnabled: true,
        hoverStateEnabled: true,
        columns: [
            {
                dataField: "name",
                caption: "Name",
                allowGrouping: false,
                //width: 150
            },
            {
                dataField: "age",
                caption: "Age",
                allowGrouping: true,
                //width: 80,
                alignment: "center"
            },
            {
                dataField: "gender",
                caption: "Gender",
                //width: 100,
                alignment: "center"
            },
            {
                dataField: "contact",
                caption: "Contact",
                //width: 150
            },
            {
                dataField: 'years',
                caption: "Total Experience",
                //width: 130,
                alignment: "center",
                cellTemplate: function (container, options) {
                    const years = options.value || 0;
                    const text = years > 0 ? `${years} years` : 'No experience';
                    $("<span>").text(text).appendTo(container);
                }
            },
            {
                dataField: 'imagePath',
                caption: "Photo",
                //width: 100,
                allowSorting: false,
                allowFiltering: false,
                cellTemplate: function (container, options) {
                    const imageFileName = options.value;
                    const imageUrl = imageFileName
                        ? `/uploads/${imageFileName}`
                        : `/uploads/defaultimage.jpg`;

                    container.css({
                        display: "flex",
                        justifyContent: "center",
                        alignItems: "center",
                        padding: "5px"
                    });

                    $("<img>")
                        .attr("src", imageUrl)
                        .attr("alt", "Employee Photo")
                        .css({
                            width: 45,
                            height: 45,
                            borderRadius: "50%",
                            objectFit: "cover",
                            border: "2px solid #ddd"
                        })
                        .on('error', function () {
                            // Fallback to default image if loading fails
                            $(this).attr('src', '/uploads/defaultimage.jpg');
                        })
                        .appendTo(container);
                }
            },
            {
                type: "buttons",
                caption: "Actions",
                //width: 120,
                buttons: [
                    {
                        hint: "Edit Employee",
                        icon: "edit",
                        onClick: function (e) {
                            const employeeId = e.row.data.employeeID;
                            editEmployee(employeeId);
                        }
                    },
                    {
                        hint: "Delete Employee",
                        icon: "trash",
                        onClick: function (e) {
                            const employeeId = e.row.data.employeeID;
                            deleteEmployee(employeeId);
                        }
                    }
                ]
            }
        ],
        filterRow: { visible: true },
        searchPanel: {
            visible: true,
            width: 240,
            placeholder: "Search employees..."
        },
        groupPanel: { visible: true },
        allowColumnReordering: true,
        allowColumnResizing: true,
        columnAutoWidth: true,
        selection: { mode: "single" },
        paging: {
            pageSize: 20
        },
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [10, 20, 50, 100],
            showInfo: true
        },

        // Add toolbar with add button
        onToolbarPreparing: function (e) {
            e.toolbarOptions.items.unshift({
                location: "after",
                widget: "dxButton",
                options: {
                    icon: "add",
                    hint: "Add Employee",
                    type: "success",
                    stylingMode: "contained",
                    onClick: function () {
                        // Use the centralized form initialization with add mode
                        initializeEmployeeForm('add');
                    }
                }
            });

            //// Add refresh button
            //e.toolbarOptions.items.push({
            //    location: "after",
            //    widget: "dxButton",
            //    options: {
            //        icon: "refresh",
            //        hint: "Refresh",
            //        onClick: function () {
            //            loadEmployee();
            //        }
            //    }
            //});
        },

        // Handle row events
        onRowPrepared: function (e) {
            if (e.rowType === "data") {
                e.rowElement.css("cursor", "pointer");
            }
        }
    }).dxDataGrid("instance");
}

// Initialize application
$(function () {
    console.log("Initializing employee management system...");

    // Load employees and initialize datagrid
    loadEmployee(function () {
        datagridinit();
        console.log("Employee management system initialized successfully");
    });
});

//// Utility function to refresh the employee list
//function refreshEmployeeList() {
//    loadEmployee(function () {
//        if (dataGrid) {
//            dataGrid.option("dataSource", EmployeeList);
//            dataGrid.refresh();
//        }
//    });
//}