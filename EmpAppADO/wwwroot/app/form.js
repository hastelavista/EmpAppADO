let experienceList = [];
let experienceCounter = 1;

// Load employee data and populate form
function loadEmployeeForm(response) {
    const employeeData = response.employee;
    const experiences = response.experiences || [];

    experienceList = experiences.map((exp, index) => ({
        ...exp,
        id: index + 1
    }));
    experienceCounter = experiences.length + 1;

    const popupInstance = $("#employeeADoPopup").dxPopup("instance");
    if (popupInstance) {
        popupInstance.show();
    }
    const formInstance = $("#employeeForm").dxForm("instance");

    if (formInstance) {
        formInstance.option("formData", employeeData);
    }

    renderExperienceGrid();
}

// Render employee form within popup
function renderForm(container) {
    $("<div>").attr("id", "employeeForm").appendTo(container).dxForm({
        formData: {},
        colCount: 2,
        items: [
            { dataField: "name", label: { text: "Name" }, isRequired: true },
            { dataField: "age", editorType: "dxNumberBox", editorOptions: { min: 18, max: 100 } },
            {
                dataField: "gender",
                editorType: "dxSelectBox",
                editorOptions: {
                    items: ["Male", "Female", "Other"],
                    value: ""
                }
            },
            { dataField: "contact" },
            {
                itemType: "group",
                colSpan: 2,
                caption: "Experiences",
                template: function (data, itemElement) {
                    const $section = $("<div>").appendTo(itemElement);

                    // Add Experience Button
                    $("<div>").dxButton({
                        text: "Add Experience",
                        icon: "add",
                        type: "success",
                        stylingMode: "contained",
                        onClick: function () {
                            showExperiencePopup();
                        }
                    }).appendTo($section);

                    // Experience Grid
                    $("<div>").attr("id", "experienceTable").appendTo($section).dxDataGrid({
                        dataSource: experienceList,
                        keyExpr: "id",
                        showBorders: true,
                        editing: {
                            mode: "row",
                            allowUpdating: true,
                            allowDeleting: true,
                            useIcons: true
                        },
                        columns: [
                            { dataField: "company", caption: "Company", validationRules: [{ type: "required" }] },
                            { dataField: "department", caption: "Department", validationRules: [{ type: "required" }] },
                            {
                                dataField: "years",
                                caption: "Years",
                                dataType: "number",
                                editorType: "dxNumberBox",
                                editorOptions: { min: 0 },
                                validationRules: [{ type: "required" }]
                            }
                        ]
                    });
                }
            },
            {
                itemType: "button",
                colSpan: 2,
                horizontalAlignment: "right",
                buttonOptions: {
                    text: "Save",
                    type: "default",
                    onClick: function () {
                        saveEmployeeForm();
                    }
                }
            },
            {
                itemType: "button",
                colSpan: 2,
                horizontalAlignment: "right",
                buttonOptions: {
                    text: "Cancel",
                    type: "danger",
                    onClick: function () {
                        $("#employeeADoPopup").dxPopup("instance").hide();
                    }
                }
            }
        ]
    });
}

// Update experience grid with current data
function renderExperienceGrid() {
    const gridInstance = $("#experienceTable").dxDataGrid("instance");
    if (gridInstance) {
        gridInstance.option("dataSource", experienceList);
    }
}

// Save employee form data
function saveEmployeeForm() {
    const formInstance = $("#employeeForm").dxForm("instance");
    const formData = formInstance.option("formData");

    const employeeData = {
        employee: formData,
        experiences: experienceList
    };
    console.log(employeeData);

    let isUpdate = employeeData.employee.employeeID && employeeData.employee.employeeID > 0;

    $.ajax({
        url: isUpdate ? "EmpView/UpdateEmp" : "/EmpView/AddNewEmp",
        type: isUpdate ? "POST" : "POST",
        data: JSON.stringify(employeeData),
        contentType: "application/json",
        success: function (response) {
            alertify.success("Employee saved successfully!");
            $("#employeeADoPopup").dxPopup("instance").hide();

            console.log("Server response:", response);

            // Reload employee list and refresh grid
            loadEmployee(function () {
                if (dataGrid) {
                    dataGrid.option("dataSource", EmployeeList);
                    dataGrid.refresh();
                }
            });
        },
        error: function (xhr) {
            alertify.error("Error saving employee.");
            console.error(xhr.responseText);
        }
    });
}

// Show experience popup
function showExperiencePopup() {
    const popupContent = $("<div>").attr("id", "experienceForm");

    popupContent.dxForm({
        formData: {
            company: "",
            department: "",
            years: 0
        },
        items: [
            { dataField: "company", label: { text: "Company" }, validationRules: [{ type: "required" }] },
            { dataField: "department", label: { text: "Department" }, validationRules: [{ type: "required" }] },
            {
                dataField: "years",
                label: { text: "Years" },
                editorType: "dxNumberBox",
                editorOptions: { min: 0 },
                validationRules: [{ type: "required" }]
            }
        ]
    });

    $("#experienceADoPopupContainer").dxPopup({
        title: "Add Experience",
        contentTemplate: () => popupContent,
        width: 400,
        height: 300,
        showCloseButton: true,
        showTitle: true,
        visible: true,
        dragEnabled: true,
        closeOnOutsideClick: true,
        toolbarItems: [
            {
                widget: "dxButton",
                toolbar: "bottom",
                location: "after",
                options: {
                    text: "Save",
                    type: "success",
                    onClick: function () {
                        const formInstance = $("#experienceForm").dxForm("instance");
                        const data = formInstance.option("formData");

                        if (!data.company || !data.department || data.years === null) {
                            alertify.error("Please fill all required fields.");
                            return;
                        }

                        experienceList.push({ ...data, id: experienceCounter++ });
                        renderExperienceGrid();

                        $("#experienceADoPopupContainer").dxPopup("instance").hide();
                        alertify.success("Experience added.");
                    }
                }
            },
            {
                widget: "dxButton",
                toolbar: "bottom",
                location: "after",
                options: {
                    text: "Cancel",
                    type: "danger",
                    onClick: function () {
                        $("#experienceADoPopupContainer").dxPopup("instance").hide();
                    }
                }
            }
        ]
    });
}

