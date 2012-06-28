var watin6 = document.getElementById('watin6');
var newEvt = document.createEventObject();newEvt.button = 1;watin6.fireEvent('OnDoubleClick', newEvt);
更新手记：
1、执行脚本时，如果有选中，只执行当前选中(张晓飞 2011.11.08);
2、脚本的循环执行问题，增加循环次数的输入框；
3、优化元素遍历、定位代码，提供执行效率；
4、修复执行时第一行不选中的Bug;
5、修复(有关调用实时(JIT)调试而不是此对话框的详细信息,张岚曹兴发现 2011.11.09
	if (wsManager == null || watinIE == null) return; //尚未初始化，直接返回
6、添加对ymPrompt对话框的支持；
7、添加对Alert,Confirm对话框的支持；
8、增加对中文输入的支持；
9、从简化用户操作考虑，合并TypeText和AppendText
10、增加ActionHight接口；
