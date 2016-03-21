前   言
> 做为一名IT人，接触NET有几年了，随着对NET的逐渐了解，看到一个很有意思的现象，就是好多平台的框架，组件很容易移植到NET平台，如Junit, Lucene,Ant,WatiR,Hibernate,Log4等等，但好多后来就沉了如NetBPM,BugNet,MBUnit, SharpDevelop,Rainbow，ZenOS等，正所谓其兴也勃，其衰也速。最令人扼腕的莫过于Mono了。这里要讨论的是另一个项目WatiN Recorder,那的创意很好，可以也没有坚持下去，2008年5月后就没有更新了，本文分析项目被放弃的原因，提出专注于用户录制、回放，模拟等核心需求，对原项目进行重构，并增加了UI测试人员熟悉的JS接口，最后提供免费下载，希望大家能用的到！
关于WatiN 和WatiN Recorder
> WatiN是一个在.NET中自动控制浏览器的开源类库。WatiN从流行于Ruby社区的Watir项目中得到启发，通过与浏览器的交互简化了对Web应用程序的自动测试。WatiN使用C#编写，但是可以使用任何.NET语言编写测试。目前它只支持Windows平台。
1、WatiN Recorder 生成兼容Wait的UI测试脚本；
2、支持生成多种语言的脚本；
3、用户可定义生成模板；
4、嵌入IE浏览器，但生成Firefox版本；
5、对Frame,Iframe的支持；
6、支持WatiN的大部分方法
7、断点和单步支持；
8、使用模拟数据支持同一个脚本
9、可查看Html DOM结构；
10、查看WatiN元素，可高亮显示
WatiN Recorder 目前的问题
> 从上面的介绍可以看出，该项目有很多亮点，但定位成生成WatiN兼容的代码，有点问题。测试人员希望用这个工具进行模拟测试，回归测试，并不关心生成的脚本是什么!因而
将精力放在生成代码了，用户评价并不好，如：
> “WatiN Test Recorder，测试代码能写还是自己写吧。这玩意儿抓出来的东西。。实在不是一个丑字能形容的啊。尽管说，不论白老鼠，黑老鼠，能吓退猫的都是牛B的老鼠。但是网页结构一旦复杂了点。自动捕获的就未必是你想要的”
> –引http://www.cnblogs.com/xiaosuo/archive/2010/06/14/1758210.html
其二，系统太多Bug,动不动就崩溃了，下载的源码也不能编译通过！这里没有任何批评的意思，我的真实体会，仔细看了代码，发现有很多值得学习的东西，因而觉得对其进行重构和改进。
我对WatiN TestRecorder的改进
> > 针对上面的问题，我希望提出方案，给NET社区做点贡献
1、专注于脚本的录制，恢复，模拟数据的嵌入；
2、简化用户操作，给用户留下扩展接口（javascript,jQuery）；
3、简化安装，修复原项目的Bug，实现部分遗留功能
4、对原项目进行重构，解除业务层与展示层的耦合；
其中1,2,3是面向用户的，4是面向开发人员的；


> 项目下载与安装
1、运行环境 net framework 2.0,IE 7.0+
2、项目下载
Csdn下载地址:
Google 项目首页：https://code.google.com/p/testrecorder-for-watin/
Svn地址：https://testrecorder-for-watin.googlecode.com/svn/trunk/
3、安装卸载 直接运行根目录下的Intall.bat,UnInstall.bat 绿色环保
> > 根目录下有个Baidu.xml的文件，是示例脚本，可直接运行。
4、在线交流与反馈 QQ:55643774  菜单->Help->FeedBack发送邮件