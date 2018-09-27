$(function(){
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
    var openid = GetQueryString('openid');
    var nickname = decodeURI(GetQueryString('nickname'));
    var headImgurl = GetQueryString('headimgurl');



    getOpenId();

    $(".okbtn").click(function(){
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
            , content: '正在抽奖中。。'
        });
        var $codenum = $("#codenum").val();
        var $address = $("#address").val(); 
        $.ajax({
            type: 'post',
            url: '/PrizeResult/C',
            data: {
                OpenID: infoWechatUserReturn.openid,
                Name: $("#username").val(),
                Tel: $("#usernum").val(),
                SnNumber: $("#codenum").val()
            },
            header: {
                    'Accept': "application/json"
            },
            async: true,
            dataType: 'json',
            success: function (data) {
                layer.closeAll();
                console.log(data);
                if (data.ErrorCode!=0) {
                    alert(data.ErrorMsg);
                    return;
                }
                if (data.Data.Result =="旅游大奖")
                {
                    $("#prize").show();
                    $("#good1").show();
                }
                else if (data.Data.Result != "")
                {
                    $("#prize").show();
                    $("#good2").show();
                    $("#vipnum").val(data.Data.Result);
                    vipnumClick();
                }
                else {
                    alert("谢谢参与，祝您好运");
                }


            },
            error: function(){
                layer.closeAll();
               alert("网络错误，请稍后再试");
            }
        }) 
    }


    function getOpenId(){
        $.ajax({
            type: 'get',
            /*data: {
                openid: openid 
            },*/
            url: 'http://www.qftang.cn/client/game/vivo_sd_1001/src/getByOpenid.php?openid='+openid,
            success: function(data){
                console.log(data); 
                if(data.d.level!='0'){
                    $("#prize").show();
                    $("#good"+data.d.level).show();
                    $("#vipnum").val(data.d.content);
                    vipnumClick();         
                }
            }
        })
    }
    
    var getAppId;
    var getNonceStr;
    var getSignature;
    var getTimestamp;
    var getUrl = encodeURIComponent(location.href.split('#')[0]);
    $.ajax({
        url: "http://www.qftang.cn/wechat/api.php?type=get_jsapi&url=" + getUrl,
        async: false,
        type: "GET",
        header: {
            'accept': 'application/json',
        },
        dataType: 'json',
        data: {},
        error: function (request) {
        },
        success: function (data) {
            getAppId = data.appId;
            getNonceStr = data.nonceStr;
            getSignature = data.signature;
            getTimestamp = data.timestamp;
        }
    });
    wx.config({
        debug: false, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
        appId: getAppId,
        timestamp: getTimestamp,
        nonceStr: getNonceStr,
        signature: getSignature,
        jsApiList: ['scanQRCode']
    });
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