function generateBuildingReport(primaryControl) {
    Xrm.Utility.showProgressIndicator("Generating report...");
    var request = {};
    request.getMetadata = function () {
        return {
            boundParameter: "entity",
            operationName: "dkl_BuildingReport",
            operationType: 0,
            parameterTypes: {
                "entity": { 
                    typeName: "dkl_building", 
                    structuralProperty: 5 
                },
                "buildingID": {
                    "typeName": "Edm.String",
                    "structuralProperty": 1 // Entity Type
                },
            },
        };
    };
    request.entity = primaryControl.data.entity.getEntityReference();
    request.buildingID = primaryControl.data.entity.getId();
    Xrm.WebApi.online.execute(request).then(
        function (result) {
            Xrm.Utility.closeProgressIndicator();
            Xrm.Page.data.refresh(true)
            var alertStrings = {
                confirmButtonLabel: "Ok",
                text:  "The report was generated successfully. Please check your email inbox.",
                title: "Success"
            };
            Xrm.Navigation.openAlertDialog(alertStrings);
        },

        function (error) {
            Xrm.Utility.closeProgressIndicator()
            var alertStrings = {
                confirmButtonLabel: "Ok",
                text: errorText + error.message,
                title: errorTitle
            };
            Xrm.Navigation.openAlertDialog(alertStrings);
    });
}