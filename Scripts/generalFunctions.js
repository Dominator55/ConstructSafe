function editableOnCreate(executionContext, fieldnames) {
    var formContext = executionContext.getFormContext();
    if(formContext.ui.getFormType()==1){
        fieldnames.forEach(fieldname => {
            var control = formContext.getControl(fieldname);
            if (control) {
                control.setDisabled(false);
            }
        });
    }
}
  
function startIsMoreThenEnd(executionContext, start, end) {
    var formContext = executionContext.getFormContext()
    var startTime = formContext.getAttribute(start).getValue()
    var endTime = formContext.getAttribute(end).getValue() 
    var startControl = formContext.getControl(start)
    var endControl = formContext.getControl(end)
    if(endTime < startTime && endTime!=null){
        startControl.setNotification("Start cannot be later then end.")
        formContext.getAttribute(start).setValue(endTime)
    }
    else{
        startControl.clearNotification()
        endControl.clearNotification()
    }
}

function endIsLessThenStart(executionContext, start, end) {
    var formContext = executionContext.getFormContext()
    var startTime = formContext.getAttribute(start).getValue()
    var endTime = formContext.getAttribute(end).getValue() 
    var startControl = formContext.getControl(start)
    var endControl = formContext.getControl(end)
    if(startTime>endTime){
        endControl.setNotification("End cannot be sooner then start.")
        formContext.getAttribute(end).setValue(startTime)
    }
    else{
        endControl.clearNotification()
        startControl.clearNotification()
    }
}