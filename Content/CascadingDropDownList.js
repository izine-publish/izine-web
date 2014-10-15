function bindDropDownList(e, targetDropDownList) 
{
    var key = this.value;
    var allOptions = targetDropDownList.allOptions;
    var option;
    var newOption;
    targetDropDownList.options.length = 0;
    
    for (var i=0; i < allOptions.length; i++) 
    {
        option = allOptions[i];
        if (option.key == key) 
        {
            newOption = new Option(option.text, option.value);
            targetDropDownList.options.add(newOption);
        }
    }
}