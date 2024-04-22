function displayBasedOnType(executionContext){
    var formContext = executionContext.getFormContext();
    var nonConformityType = formContext.getAttribute('dkl_type').getValue();
    if(nonConformityType==777490004){
        formContext.getControl('dkl_articleviolated').setVisible(true);
    }
    else{
        formContext.getControl('dkl_articleviolated').setVisible(false);
    }
}