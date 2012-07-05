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
var MaxRequest = 20;
var MaxPageSize = 10000;
var TotalNum = 0;
var Requests = {};
var Start = new Date();


//设置定时器，查询未使用的
var timer = setInterval(function() {
    var count = getPropertyCount(Requests);
    if (count < MaxRequest) getUnUsed(10);
}, 250);

var timer = setInterval(function() {
    var rate = getTimeSpan(Start);
    if (rate == 0) rate = "0.0";
    if (rate != "0.0") rate = (TotalNum / rate).toFixed(2);

    var count = getPropertyCount(Requests);
    console.log('concurrent request=' + count + ',send=' + Send + ',recive=' + Recieve + ',total=' + TotalNum + ',rate=' + rate + '/s');
}, 3000);
 
//取得未使用的
function getUnUsed(num) {
    var option = {
        host: "www.ccdzfw.gov.cn",
        port: 80,
        path: "/qq/json.aspx?mod=qq&act=getunused&num=" + num
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
        host: "city.pengyou.com",
        port: 80,
        base: "/json.php?mod=friend&act=getfrisforguest&group=0&g_tk=123abc&uin=",
        path: "",
        uin: uin,
        page: page || 1,
        time: new Date()
    };
    options.path = options.base + options.uin + "&page=" + options.page;
    
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
        console.log("request to city.pengyou.com error: " + e.message + ' ,Timspan=' + getTimeSpan(options.time));
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
    var root = eval('(' + content + ')');
    if (root.data && root.data.ret) {

        var total = parseInt(root.data.totalPages);
        if (page == "1" && total > 1) {
            for (var index = 2; index <= total; index++) {
                sendRequest(uin, index);
            }
        }
        var paths = [];
        for (var id in root.data.ret) {
            Actors[id] = root.data.ret[id];
        }

        var len = getPropertyCount(Actors);
        if (len > MaxPageSize && IsSaving == false) { //======防多次保存========
            IsSaving = true;
            TotalNum += len;
            var jsonStr = JSON.stringify(Actors);
            var filename = 'data/' + (new Date()).getTime() + ".log";

            fs.writeFile(filename, jsonStr, function(err) {
                if (err) console.log("open file error：" + err); 
                
                IsSaving = false;
                Actors = {};
                console.log('saving file to ' + filename + ' success!');
            });
        }
    } else {
        if (root.err)
            console.log('handleResponse error=' + root.err);
        else
            console.log('handleResponse error=' + content);
    }
};
