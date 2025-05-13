
var EmployeeList = [];
var dataGrid;

// Load employee list from server
function loadEmployee(onSuccess) {
    $.ajax({
        url: 'https://localhost:7167/api/Emp/all',
        type: 'GET',
        success: function (res) {
            EmployeeList = res;

            // Call onSuccess callback if provided
            if (typeof onSuccess === 'function') {
                onSuccess();
            }

            // Refresh grid if it exists
            if (dataGrid) {
                dataGrid.option("dataSource", EmployeeList); // Update data source explicitly
                dataGrid.refresh();
            } else {
                console.error("datagrid is not initialized");
            }
        },
        error: function (err) {
            alertify.error("Error loading employees");
            console.log(err);
        }
    });
}

// Initialize datagrid
function datagridinit() {
    dataGrid = $("#dataGrid").dxDataGrid({
        dataSource: EmployeeList,
        keyExpr: "employeeID",
        columnFixing: { enabled: true },
        columns: [
            { dataField: "name", caption: "नाम", allowgrouping: false },
            { dataField: "age", allowgrouping: true },
            { dataField: "gender" },
            { dataField: "contact" },
            { dataField: 'years' },
            {
                type: "buttons",
                buttons: [
                    {
                        hint: "Edit",
                        icon: "edit",
                        onClick: function (e) {
                            const id = e.row.data.employeeID;

                            $("#employeeADoPopup").dxPopup({
                                title: "Edit Employee",
                                visible: true,
                                width: 700,
                                height: "auto",
                                showTitle: true,
                                dragEnabled: true,
                                closeOnOutsideClick: false,
                                contentTemplate: function (contentElement) {
                                    renderForm(contentElement);

                                    $.ajax({
                                        url: `https://localhost:7167/api/Emp/${id}`,
                                        type: 'GET',
                                        success: function (response) {
                                            console.log("RESULT", response);
                                            loadEmployeeForm(response);
                                        }
                                    });
                                }
                            }).dxPopup("instance");
                        }
                    },
                    {
                        hint: "Delete",
                        icon: "trash",
                        onClick: function (e) {
                            const empId = e.row.data.employeeID;

                            alertify.confirm("Confirm Deletion", "Are you sure you want to delete this employee?",
                                function () {
                                    $.ajax({
                                        url: `https://localhost:7167/api/Emp/${empId}`,
                                        type: 'DELETE',
                                        success: function () {
                                            alertify.success("Employee Deleted");
                                            loadEmployee(function () {
                                                if (dataGrid) {
                                                    dataGrid.option("dataSource", EmployeeList);
                                                    dataGrid.refresh();
                                                }
                                            });
                                        },
                                        error: function () {
                                            alertify.error("Delete failed.");
                                        }
                                    });
                                },
                                function () {
                                    alertify.message('Canceled');
                                }
                            );
                        }
                    }
                ]
            }
        ],
        filterRow: { visible: true },
        searchPanel: { visible: true },
        groupPanel: { visible: true },
        allowColumnReordering: true,
        allowColumnResizing: true,
        columnAutoWidth: true,
        selection: { mode: "single" },

        // Add button in toolbar
        onToolbarPreparing: function (e) {
            e.toolbarOptions.items.unshift({
                location: "after",
                widget: "dxButton",
                options: {
                    icon: "add",
                    text: "",
                    type: "default",
                    onClick: function () {
                        experienceList = [];
                        experienceCounter = 1;

                        $("#employeeADoPopup").dxPopup({
                            title: "Add Employee",
                            visible: true,
                            width: 700,
                            height: "auto",
                            showTitle: true,
                            dragEnabled: true,
                            closeOnOutsideClick: false,
                            contentTemplate: function (contentElement) {
                                renderForm(contentElement);
                            }
                        }).dxPopup("instance");
                    }
                }
            });
        },
    }).dxDataGrid("instance");
}

// Initialize application
$(function () {
    // Load employees and initialize datagrid
    loadEmployee(datagridinit);
});