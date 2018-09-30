
function checkAll(allName, sName) {
    var IsChecked = $("#" + allName).prop('checked');
    console.log(IsChecked);
    if (IsChecked) {
        $("[name='" + sName + "']").prop("checked", 'true');//全选
        console.log($("[name='" + sName + "']").prop("checked"));
    }
    else {
        $("[name='"+sName+"']").removeAttr("checked");
    }
}
