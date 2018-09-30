var ErrorJson = [
    { "ID": -1, "CN": "合法，抽过奖，显示出之前的信息", "EN": "SN has participated in the lottery." },
    { "ID": 1, "CN": "用户名不能为空", "EN": "User name can not be empty." },
    { "ID": 2, "CN": "用户名不能超过50个字符", "EN": "The user name should not exceed 50 characters." },
    { "ID": 3, "CN": "地址不能为空", "EN": "The Store address must not be empty." },
    { "ID": 4, "CN": "地址不能超过500", "EN": "" },
    { "ID": 5, "CN": "手机号输入不正确，请重新输入", "EN": "Incorrect phone number input." },
    { "ID": 6, "CN": "SN码长度应该是15位", "EN": "The length of SN code is incorrect." },
    { "ID": 7, "CN": "未选择区域", "EN": "Unselected region" },
    { "ID": 8, "CN": "SN码非法，在数据库不存在", "EN": "SN code is illegal" },
    { "ID": 9, "CN": "数据未保存，要重试", "EN": "Please Try Again" },
    { "ID": 10, "CN": "", "EN": "1" },
    { "ID": 11, "CN": "", "EN": "2" },
    { "ID": 12, "CN": "", "EN": "3" },
    { "ID": 13, "CN": "", "EN": "4" },
    { "ID": 14, "CN": "", "EN": "5" },
];


$(function () {
    $(".rulebtn").click(function(){
        $("#rule").show();
        $("#info").hide();
    })
    $(".wrapperbtn").click(function(){
        $("#info").show();
        $("#wrapper").hide();
        $("body").css("background","#5e9fd9");
    })

    function GetQueryString(name) { //获取url的参数
        var url = window.location.search; //获取url中"?"符后的字串
        var theRequest = new Object();
        if (url.indexOf("?") != -1) {
            var str = url.substr(1);
            strs = str.split("&");
            for(var i = 0; i < strs.length; i ++) {
                //就是这句的问题
                theRequest[strs[i].split("=")[0]]=decodeURI(strs[i].split("=")[1]);
                //之前用了unescape()
                //才会出现乱码
            }
        }
        return theRequest[name];
    }




    $(".okbtn").click(function () {
        showWinningstate();
        return;
        var $username = $("#username").val();
        var $usernum = $("#usernum").val();
        var $codenum = $("#codenum").val();
        if($username == ''){
            alert("用户名不能为空，请输入重试");
        }else if($usernum == ''){
            alert("电话号码不能为空，请输入重试");
        }else if($codenum == ''){
            alert("串码不能为空，请输入重试");
        }else{
            var reg = /[^0-9]/g;
            var len = $usernum.length;
            if(reg.test($usernum)){
                alert("手机号输入不正确，请重新输入")
            }else{
                showWinningstate();
            }
            
        }
        
    })

    $("#close").click(function(){
        $("#rule").hide();
        $("#info").show();
    })

    $(".sure").click(function(){
        $("#prize").hide();
        $(".goods").hide();
    })
    

    function vipnumClick(){
        var clipboard = new Clipboard('.copy');
        clipboard.on('success',function(e){
            alert("复制成功");
            e.clearSelection();
        });
        clipboard.on('error',function(e){
            alert("复制失败，请选择“拷贝”复制");
        })
        
    }


    function showWinningstate() {
        layer.open({
            type: 2
            , content: 'Processing。。'
        });
        $.ajax({
            type: 'post',
            url: '/PrizeResult/C',
            data: {
                Name: $("#TxtName").val(),
                StoreAdd: $("#TxtAddress").val(),
                Tel: $("#TxtTel").val(),
                SnNumber: $("#TxtSnNumber").val(),
                AreaName: $("#TxtAreaName").val()
            },
            header: {
                    'Accept': "application/json"
            },
            async: true,
            dataType: 'json',
            success: function (data) {
                layer.closeAll();
                console.log(data);
                if (data.ErrorCode==0) {
                    $("#prize").show();
                    $("#good" + data.Data.Result).show();
                    return;
                }
                else {
                    var HasErrorBeenFind = false;
                    $(ErrorJson).each(function (indexJson,itemJson) {
                        if (itemJson.ID == data.ErrorCode) {
                            HasErrorBeenFind = true;
                            alert(itemJson.EN);
                            return true;
                        }
                    });
                    if (!HasErrorBeenFind) {
                        alert("Undefind Error");
                    }
                }
                


            },
            error: function(){
                layer.closeAll();
               alert("Network Error,Please Try Again Later");
            }
        }) 
    }


    
    $(".codebtn").click(function () {
        wx.scanQRCode({
            needResult: 1, // 默认为0，扫描结果由微信处理，1则直接返回扫描结果，
            scanType: ["qrCode", "barCode"], // 可以指定扫二维码还是一维码，默认二者都有
            success: function (res) {
                console.log(res.resultStr)
                // 当needResult 为 1 时，扫码返回的结果
                var arr = res.resultStr.split(",");
                $("#codenum").val(arr[1]);
            }
        });
    });



    /////////////////////////////////////////////////////去除alert顶部域名
    window.alert = function(name){
        var iframe = document.createElement("IFRAME");
        iframe.style.display="none";
        iframe.setAttribute("src", 'data:text/plain,');
        document.documentElement.appendChild(iframe);
        window.frames[0].window.alert(name);
        iframe.parentNode.removeChild(iframe);
    }

})