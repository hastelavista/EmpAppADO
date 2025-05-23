// Global variables
let experienceList = [];
let experienceCounter = 1;
let currentFormMode = 'add'; // 'add' or 'edit'
let currentEmployeeData = null;

// Initialize form with mode-specific behavior
function initializeEmployeeForm(mode = 'add', employeeData = null) {
    currentFormMode = mode;
    currentEmployeeData = employeeData;

    // Reset experience data for add mode
    if (mode === 'add') {
        experienceList = [];
        experienceCounter = 1;
    }

    const popupTitle = mode === 'add' ? 'Add Employee' : 'Edit Employee';

    $("#employeeADoPopup").dxPopup({
        title: popupTitle,
        visible: true,
        width: 500,
        height: "auto",
        showTitle: true,
        dragEnabled: true,
        closeOnOutsideClick: false,
        contentTemplate: function (contentElement) {
            renderForm(contentElement);

            // Load data for edit mode
            if (mode === 'edit' && employeeData) {
                loadEmployeeFormData(employeeData);
            }
        }
    }).dxPopup("instance");
}

// Load employee data for edit mode
function loadEmployeeFormData(response) {
    const employeeData = response.employee;
    const experiences = response.experiences || [];

    // Setup experience data
    experienceList = experiences.map((exp, index) => ({
        ...exp,
        id: index + 1
    }));
    experienceCounter = experiences.length + 1;

    // Populate form
    const formInstance = $("#employeeForm").dxForm("instance");
    if (formInstance) {
        formInstance.option("formData", employeeData);

        // Update image preview for edit mode
        setTimeout(() => {
            updateImagePreview(employeeData.imagePath);
        }, 100);
    }

    renderExperienceGrid();
}

// Update image preview based on mode
function updateImagePreview(imagePath) {
    const $imagePreview = $("#photo-preview");
    const $removeButton = $("#remove-photo-btn");

    if (currentFormMode === 'edit' && imagePath) {
        const imageUrl = imagePath.startsWith("/uploads/") ? imagePath : `/uploads/${imagePath}`;
        $imagePreview.attr("src", imageUrl).show();
        $removeButton.show();
    } else if (currentFormMode === 'add') {
        $imagePreview.hide();
        $removeButton.hide();
    }
}

// Handle file selection
function handleFileSelection(file) {
    const $imagePreview = $("#photo-preview");
    const $removeButton = $("#remove-photo-btn");

    if (file) {
        const reader = new FileReader();
        reader.onload = function (event) {
            $imagePreview.attr("src", event.target.result).show();
            $removeButton.show();
        };
        reader.readAsDataURL(file);
    }
}

// Remove photo handler
function removePhoto() {
    const $imagePreview = $("#photo-preview");
    const $removeButton = $("#remove-photo-btn");
    const uploaderInstance = $("#file-uploader").dxFileUploader("instance");

    $imagePreview.hide();
    $removeButton.hide();
    uploaderInstance.reset();
}

// Render employee form
function renderForm(container) {
    $("<div>").attr("id", "employeeForm").appendTo(container).dxForm({
        formData: {},
        colCount: 2,
        minColWidth: 300, 
        colCountByScreen: { xs: 2, sm: 2, md: 2, lg: 2 },
        height: 'auto',
        labelLocation: "top", 
        items: [
            {
                dataField: "name",
                label: { text: "Name" },
                isRequired: true
            },
            { 
                dataField: "age",
                label: { text: "Age" },
                editorType: "dxNumberBox",
                editorOptions: { min: 18, max: 100 }
            },
            {
                dataField: "gender",
                label: { text: "Gender" },
                editorType: "dxSelectBox",
                editorOptions: {
                    items: ["Male", "Female", "Other"],
                    value: ""
                }
            },
            {
                dataField: "contact",
                label: { text: "Contact" }
            },
            {
                dataField: "imagepath",
                itemType: "simple",
                colSpan: 2,
                label: { text: "Photo" },
                template: function (data, itemElement) {
                    createPhotoSection(itemElement);
                }
            },
            {
                itemType: "group",
                colSpan: 2,
                caption: "Work Experience",
                template: function (data, itemElement) {
                    createExperienceSection(itemElement);
                }
            },
            ...createActionButtons()
        ]
    });
}

// Create photo upload section with mode-specific behavior
function createPhotoSection(itemElement) {
    const container = $("<div>").css({
        display: "flex",
        alignItems: "flex-start",
        gap: "5px",
        flexWrap: "wrap"
    }).appendTo(itemElement);

    // Image preview (hidden initially for add mode)
    const $imagePreview = $("<img>")
        .attr("id", "photo-preview")
        .css({
            width: "80px",
            height: "80px",
            objectFit: "cover",
            borderRadius: "8px",
            border: "2px solid #ddd",
            display: currentFormMode === 'add' ? "none" : "block"
        })
        .appendTo(container);

    // Controls container
    const controlsContainer = $("<div>").css({
        display: "flex",
        flexDirection: "row",
        gap: "10px",
        alignItems: "flex-start",
        //alignItems: "center"

    }).appendTo(container);

    // File uploader
    $("<div>")
        .attr("id", "file-uploader")
        .appendTo(controlsContainer)
        .dxFileUploader({
            selectButtonText: "Select",
            labelText: "",
            accept: "image/*",
            uploadMode: "useForm",
            name: "ImageFile",
            inputAttr: { "aria-label": "Select" },
            onValueChanged: function (e) {
                const file = e.value[0];
                handleFileSelection(file);
            }
        });

    // Remove button (hidden initially for add mode)
    $("<div>")
        .attr("id", "remove-photo-btn")
        .css("display", currentFormMode === 'add' ? "none" : "block",)
        .dxButton({
            text: "Remove Photo",
            type: "normal",
            stylingMode: "outlined",
            onClick: removePhoto
        })
        .appendTo(controlsContainer);

    // Show additional info for edit mode
    if (currentFormMode === 'edit') {
        $("<small>").text("Select New")
            .css({ color: "#666", fontSize: "12px" })
            .appendTo(controlsContainer);
    }
}

// Create experience section
function createExperienceSection(itemElement) {
    const $section = $("<div>").appendTo(itemElement);

    // Add Experience Button
    $("<div>").dxButton({
        text: "Add Experience",
        icon: "add",
        type: "success",
        stylingMode: "contained",
        onClick: showExperiencePopup
    }).appendTo($section);

    // Experience Grid
    $("<div>").attr("id", "experienceTable").css({ marginTop: "15px" }).appendTo($section).dxDataGrid({
        dataSource: experienceList,
        keyExpr: "id",
        showBorders: true,
        noDataText: "No work experience added yet",
        editing: {
            mode: "row",
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true
        },
        columns: [
            {
                dataField: "company",
                caption: "Company",
                validationRules: [{ type: "required" }]
            },
            {
                dataField: "department",
                caption: "Department",
                validationRules: [{ type: "required" }]
            },
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

// Create action buttons with mode-specific text
function createActionButtons() {
    const saveButtonText = currentFormMode === 'add' ? 'Add' : 'Update';

    return [
        {
            itemType: "group",
            colSpan: 2,
            template: function (_, itemElement) {
                $("<div>")
                    .css({
                        display: "flex",
                        justifyContent: "flex-end",
                        gap: "10px",
                        width: "100%",
                        marginTop: "10px"
                    })
                    .append(
                        $("<div>").dxButton({
                            text: saveButtonText,
                            type: "success",
                            stylingMode: "contained",
                            onClick: saveEmployeeForm
                        }),
                        $("<div>").dxButton({
                            text: "Cancel",
                            type: "normal",
                            stylingMode: "outlined",
                            onClick: function () {
                                $("#employeeADoPopup").dxPopup("instance").hide();
                            }
                        })
                    )
                    .appendTo(itemElement);
            }
        }
    ];
}

// Save employee form data
function saveEmployeeForm() {
    const formInstance = $("#employeeForm").dxForm("instance");
    const formData = formInstance.option("formData");

    // Validate required fields
    if (!formData.name || !formData.name.trim()) {
        alertify.error("Please enter employee name");
        return;
    }

    const fileUploader = $("#file-uploader").dxFileUploader("instance");
    const file = fileUploader.option("value")[0];

    const formPayload = new FormData();

    // Add employee data
    Object.keys(formData).forEach(key => {
        if (formData[key] !== null && formData[key] !== undefined) {
            formPayload.append(`Employee.${key}`, formData[key]);
        }
    });

    // Add experience data
    experienceList.forEach((exp, index) => {
        Object.keys(exp).forEach(key => {
            if (key !== 'id' && exp[key] !== null && exp[key] !== undefined) {
                formPayload.append(`Experiences[${index}].${key}`, exp[key]);
            }
        });
    });

    // Add file if selected
    if (file) {
        formPayload.append("ImageFile", file);
    }

    const isUpdate = currentFormMode === 'edit';
    const url = isUpdate ? "EmpView/UpdateEmp" : "/EmpView/AddNewEmp";
    const successMessage = isUpdate ? "Employee updated successfully!" : "Employee added successfully!";

    $.ajax({
        url: url,
        type: "POST",
        data: formPayload,
        processData: false,
        contentType: false,
        success: function (response) {
            alertify.success(successMessage);
            $("#employeeADoPopup").dxPopup("instance").hide();

            // Reload employee list
            if (typeof loadEmployee === 'function') {
                loadEmployee(function () {
                    if (dataGrid) {
                        dataGrid.option("dataSource", EmployeeList);
                        dataGrid.refresh();
                    }
                });
            }
        },
        error: function (xhr) {
            const errorMessage = currentFormMode === 'add' ? "Error adding employee." : "Error updating employee.";
            alertify.error(errorMessage);
            console.error(xhr.responseText);
        }
    });
}

// Update experience grid
function renderExperienceGrid() {
    const gridInstance = $("#experienceTable").dxDataGrid("instance");
    if (gridInstance) {
        gridInstance.option("dataSource", experienceList);
        gridInstance.refresh();
    }
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
            {
                dataField: "company",
                label: { text: "Company" },
                validationRules: [{ type: "required" }]
            },
            {
                dataField: "department",
                label: { text: "Department" },
                validationRules: [{ type: "required" }]
            },
            {
                dataField: "years",
                label: { text: "Years of Experience" },
                editorType: "dxNumberBox",
                editorOptions: { min: 0, step: 0.5 },
                validationRules: [{ type: "required" }]
            }
        ]
    });

    $("#experienceADoPopupContainer").dxPopup({
        title: "Add Work Experience",
        contentTemplate: () => popupContent,
        width: 400,
        height: 'auto',
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
                    text: "Add",
                    type: "success",
                    stylingMode: "contained",
                    onClick: function () {
                        const formInstance = $("#experienceForm").dxForm("instance");
                        const data = formInstance.option("formData");

                        if (!data.company?.trim() || !data.department?.trim() || data.years <= 0) {
                            alertify.error("Please fill all required fields correctly.");
                            return;
                        }

                        experienceList.push({ ...data, id: experienceCounter++ });
                        renderExperienceGrid();

                        $("#experienceADoPopupContainer").dxPopup("instance").hide();
                        alertify.success("Experience added successfully.");
                    }
                }
            },
            {
                widget: "dxButton",
                toolbar: "bottom",
                location: "after",
                options: {
                    text: "Cancel",
                    type: "normal",
                    stylingMode: "outlined",
                    onClick: function () {
                        $("#experienceADoPopupContainer").dxPopup("instance").hide();
                    }
                }
            }
        ]
    });
}