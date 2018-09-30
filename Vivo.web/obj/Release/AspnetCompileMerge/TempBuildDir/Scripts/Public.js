
function Request(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

//判断当前浏览器是不是ie
function IsIE()
{ return ! -[1, ]; }

//设置cookies
function setCookie(c_name, value, expiredays) {
    var exdate = new Date()
    exdate.setDate(exdate.getDate() + expiredays)
    document.cookie = c_name + "=" + escape(value) +
((expiredays == null) ? "" : ";expires=" + exdate.toGMTString())
}

//获取cookies
function getCookie(c_name) {
    if (document.cookie.length > 0) {
        c_start = document.cookie.indexOf(c_name + "=")
        if (c_start != -1) {
            c_start = c_start + c_name.length + 1
            c_end = document.cookie.indexOf(";", c_start)
            if (c_end == -1) c_end = document.cookie.length
            return unescape(document.cookie.substring(c_start, c_end))
        }
    }
    return ""
}

function Layer(URL,Title,ReloadParentWhenClose) {
   
    if (!Title) {
        Title='      ';
    }
       var width = "100%";
       var heigth = "100%";

    console.log(" excute");
    layer.open({
        type: 2,
        title: Title,
        shadeClose: true,
        shade: true,
        area: [width, heigth],
        maxmin: false, //开启最大化最小化按钮
        content: URL,
        success: function (layero, index) {
            layer.full(index);
        },
        cancel: function (index) {
            if ( ReloadParentWhenClose==true) {
                try {
                    location.reload();
                } catch (e) {

                }
            }
        }
    });
}
function LayerCustomertSize(URL, Title, ReloadParentWhenClose,_width,_height) {

    if (!Title) {
        Title = '      ';
    }
    var width = _width;
    var heigth = _height;

    console.log(" excute");
    layer.open({
        type: 2,
        title: Title,
        shadeClose: true,
        shade: true,
        area: [width, heigth],
        maxmin: false, //开启最大化最小化按钮
        content: URL,
        success: function (layero, index) {
        },
        cancel: function (index) {
            if (ReloadParentWhenClose == true) {
                try {
                    location.reload();
                } catch (e) {

                }
            }
        }
    });
}

///搜索下拉框默认值
var DropDownListOptionDefaultSch = "<option value='0'>不限</option>";
///表单下拉框默认值
var DropDownListOptionDefaultForm = "<option value='0'>请选择</option>";

function APINormalGet(URL, RedirectURL) {
    console.log("使用封装的接口请求数据,请求的URL=");
    console.log(URL);
    console.log("打算请坟成功后跳转地址=");
    console.log(RedirectURL);
    $.post(URL, function (data) {
        console.log("请求得到的数据=");
        console.log(data);
        alert(data.ErrorMsg);
        if (data.ErrorCode == 0 && RedirectURL)
        {
            if (""==RedirectURL) {
                location.href = location.href;
                return;
            }
            location.href = RedirectURL;
        }
    });
}