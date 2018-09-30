function HideTableColumn(tableID,HideColumnOjb) {
    $(tableID).find('tr').find("td:eq(" + colnum.toString() + ")").hide();
    var index=-1;
    $(tableID).find("tr:eq(0)").find("th").each(function(i,v){
        if($(this).text().trim()=="客户姓名"){
            index=i;//找到列对应行索引
            return false;
        }
    });

    if(index>-1){
        $(tableID).find("tr").each(function(i,v){
            if(i>0){
                $(v).find("td").eq(index).hide();   
            }
        });
    }
}

function InitItemVisiable(IsVisable, EleItem)
{
    if (IsVisable.toLowerCase()=="false") {
        $(EleItem).hide();
    }
}