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
var MaxRequest = 8;
var MaxPageSize = 1000;
var TotalNum = 0;
var CurrentNum = 0;
var Requests = {};
var Start = new Date();
var Index=10000;

//sendRequest(10001,1);
//设置定时器，查询未使用的

var timer = setInterval(function() {
    var count = getPropertyCount(Requests);
    if (count < MaxRequest) getUnUsed(100);
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
        var filename = 'data/' + (new Date()).getTime() + ".txt";
        fs.writeFile(filename, jsonStr, function(err) {
            if (err) throw err; //console.log('saving file to ' + filename + ' success!,total=' + TotalNum);
            IsSaving = false;
            Actors = {};
            CurrentNum = 0;
        });
    }
}
, 1000);
function getUnUsed(num){
    for(var i=0;i<num;i++){
	var uin=++Index;
        sendRequest(uin,1);
    }
}
//发送请求
function sendRequest(uin, page) {
    ++Send;
    var options = {
        host: "r.qzone.qq.com",
        port: 80,
        path: "/cgi-bin/user/cgi_personal_card?remark=0&g_tk=1501396754&uin=",
        uin: uin || '93641877',
        headers: {
            'Cookie': '__Q_w_s__QZN_PYCnt=1; pvid=4208502325; __Q_w_s_hat_seed=1; __Q_w_s__QZN_MailCnt=1;',
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
	//console.log(jsonstr);

        var root = eval('(' + jsonstr + ')');
        if (root && root.uin && (root.nickname || root.title)) {
                ++CurrentNum;
		++TotalNum;
                Actors[root.uin] = root;
        }else{
            //console.log("response  warn, uin="+uin+","  + '[' + content + ']');
        }
    }
    catch (e) {
        console.log("eval error: " + e.message + '[' + content + ']');
    }
};
