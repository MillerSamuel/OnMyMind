const submitHandler=(formId,inputId)=>{
    var form=document.getElementById(formId);
    var input=document.getElementById(inputId);
    input.value=input.value.trim();
    if(input.value!=""){
        form.submit();
    }
}
