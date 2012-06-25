var $ = {};
$.each = function(object, callback, args) {
    var name, i = 0, length = object.length, isObj = length;
    if (args) {
        if (isObj) {
            for (name in object) {
                if (callback.apply(object[name], args) === false) break;
            }
        } else {
            for (; i < length; ) {
                if (callback.apply(object[i++], args) === false) break;
            }
        }
    } else {
        if (isObj) {
            for (name in object) {
                if (callback.call(object[name], name, object[name]) === false) break;
            }
        } else {
            for (; i < length; ) {
                if (callback.call(object[i], i, object[i++]) === false) break;
            }
        }
    }
    return object;
};

function getPropertyCount(obj) {
    var count = 0;
    for (var i in obj) {
        if (obj.hasOwnProperty(i)) count++;
    }
    return count;
};

var http = require("http");
var fs = require("fs");
var path = require('path');

http.globalAgent.maxSockets = 128;

var Actors = {};
var Send = 0;
var Recieve = 0;
var IsSaving = false;
var MaxFileNum = 7000;
var MaxRequest = 5;
var MaxPageSize = 5000;
var TotalNum = 0;
var CurrentNum = 0;
var Requests = {};
var Start = new Date();


//设置定时器，查询未使用的
var timer = setInterval(function() {
    var count = getPropertyCount(Requests);
    if (count < MaxRequest) getUnUsed(10);
}, 100);

var timer = setInterval(function() {
    var rate = getTimeSpan(Start);
    if (rate == 0) rate = "0.0";
    if (rate != "0.0") rate = (TotalNum / rate).toFixed(2);

    var count = getPropertyCount(Requests);
    console.log('concurrent request=' + count + ',send=' + Send + ',recive=' + Recieve + ',total=' + TotalNum + ',current='+CurrentNum +',rate=' + rate + '/s');

    if (CurrentNum > MaxPageSize && IsSaving == false) { //==========保存数据===========
        IsSaving = true;
        var jsonStr = JSON.stringify(Actors);
        var filename = 'data/' + (new Date()).getTime() + ".log";
        fs.writeFile(filename, jsonStr, function(err) {
            if (err) throw err;
            //console.log('saving file to ' + filename + ' success!,total=' + TotalNum);
            IsSaving = false;
            Actors = {};
            CurrentNum = 0;
        });
    }
}
, 1000);

//取得未使用的
function getUnUsed(num) {
    var option = {
        host: "www.ccdzfw.gov.cn",
        port: 80,
        path: "/qq/Json.aspx?mod=uin&act=getunused&num=" + num
    };
    var bufs = [];
    var qq = http.get(option, function(response) {
        response.on("data", function(data) {
            bufs.push(data);
        });
        response.on('end', function() {
            var uins = bufs.join('');
            try {
                var ret = eval('(' + uins + ')');
                if (ret.errno == '0') {
                    $.each(ret.items, function(i, v) { sendRequest(v, 1); });
                }
            } catch (e) {
                console.log("eval error: " + e.message + '[' + uins.substr(0, 1000) + ']');
            }
        });
    }).on("error", function(e) {
        console.log("getUnUsed error: " + e.message + '->' + option.path);
    });
};
//发送请求
function sendRequest(uin, page) {
    ++Send;
    var options = {
        host: "g.qzone.qq.com",
        port: 80,
        path: "/cgi-bin/friendshow/cgi_get_visitor_simple?mask=3&clear=1&sd=92860&g_tk=1720405857&uin=",
        uin: uin || '93641877',
        headers: {
            'Cookie': '__Q_w_s_hat_seed=1; __Q_w_s__QZN_TodoMsgCnt=1; __Q_w_s__QZN_MailCnt=1; __Q_w_s__appDataSeed=1; pvid=5001284318; __Q_w_s__QZN_PYCnt=2; o_cookie=55643774; pgv_pvid=5001284318; __Q_w_s_wgt_sd=1; RK=bkTy+Ps2Tp; pt2gguin=o2419514474; uin=o2419514474; skey=@Iw4kjdauy; ptisp=cm; show_id=; ptui_loginuin=2419514474; login_time=C7E9227D834DFB1647930034E3390F910B4CD73C0AEC2A2D; Loading=Yes; pgv_info=ssid=s9863286720;',
            'Accept-Charset': 'utf-8,GBK;q=0.7,*;q=0.3'
        },
        time: new Date()
    };
    options.path = options.path + options.uin;

    var buffers = [];
    var req = http.get(options, function(res) {
        res.on("data", function(chunk) {
            buffers.push(chunk);
        });
        res.on('end', function() {
            ++Recieve;
            delete Requests[options.time];
            var content = buffers.join("");
            handleResponse(options.uin, options.page, content);
        });
    }).on("error", function(e) {
        ++Recieve;
        delete Requests[options.time];
        console.log("request to g.qzone.qq.com error: " + e.message + ' ,Timspan=' + getTimeSpan(options.time));
    });

    //将并发请求缓存到字段里
    Requests[options.time] = req;
};

function getTimeSpan(date1) {
    var date3 = (new Date()).getTime() - date1.getTime()  //时间差的毫秒数

    var days = Math.floor(date3 / (24 * 3600 * 1000)) //计算出相差天数
    var leave1 = date3 % (24 * 3600 * 1000)           //计算天数后剩余的毫秒数
    var hours = Math.floor(leave1 / (3600 * 1000))    //计算相差分钟数
    var leave2 = leave1 % (3600 * 1000)        //计算小时数后剩余的毫秒数
    var minutes = Math.floor(leave2 / (60 * 1000))

    var leave3 = leave2 % (60 * 1000)      //计算分钟数后剩余的毫秒数
    var seconds = Math.round(leave3 / 1000)

    return Math.round(date3 / 1000); //alert(" 相差 " + days + "天 " + hours + "小时 " + minutes + " 分钟" + seconds + " 秒")
};
//处理响应
function handleResponse(uin, page, content) {
    try {
        var jsonstr = content.substr(11, content.length - 14);
        var root = eval('(' + jsonstr + ')');
        if (root && root.items) {
            $.each(root.items, function(i, v) {
                ++TotalNum;
                ++CurrentNum;
                Actors[v.uin] = v;
            });
        }else{
            console.log("response  warn, uin="+uin+","  + '[' + content + ']');
        }
    }
    catch (e) {
        console.log("eval error: " + e.message + '[' + content + ']');
    }
};
