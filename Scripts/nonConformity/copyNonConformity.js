function copyNonConformity(primaryControl) {
    Xrm.Utility.showProgressIndicator("Copying Non-Conformity...");
    var request = {};
    request.getMetadata = function () {
        return {
            boundParameter: "entity",
            operationName: "dkl_CopyNonConformity",
            operationType: 0,
            parameterTypes: {
                "entity": { 
                    typeName: "dkl_nonconformity", 
                    structuralProperty: 5 
                },
                "nonConformityId": {
                    "typeName": "Edm.String",
                    "structuralProperty": 1 
                },
            }
        };
    };
    request.entity = primaryControl.data.entity.getEntityReference();
    request.nonConformityId = primaryControl.data.entity.getId();
    Xrm.WebApi.online.execute(request).then(
        function (result) {
            Xrm.Utility.closeProgressIndicator();
            Xrm.Page.data.refresh(true)
            var alertStrings = {
                confirmButtonLabel: "Ok",
                text:  "The Non-Conformity was copied sucessfuly.",
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