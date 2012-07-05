<%@ Page Language="C#" AutoEventWireup="true"%>
<!DOCTYPE html>
<html lang="zh">
<head>
    <meta charset="utf-8">
    <title>Bootstrap, from Twitter</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <!-- Le styles -->
    <link href="css/bootstrap.css" rel="stylesheet">
    <link href="css/bootstrap-responsive.css" rel="stylesheet">
    <style type="text/css">
       body
        {
            padding-top: 60px;
            padding-bottom: 40px;
        }
    </style>
    
    <!-- Le HTML5 shim, for IE6-8 support of HTML5 elements -->
    <!--[if lt IE 9]>
      <script src="http://html5shim.googlecode.com/svn/trunk/html5.js"></script>
    <![endif]-->
    <!-- Le fav and touch icons -->
</head>
<body>
    <div class="navbar navbar-fixed-top">
        <div class="navbar-inner">
            <div class="container">
                <a class="btn btn-navbar" data-toggle="collapse" data-target=".nav-collapse"><span
                    class="icon-bar"></span><span class="icon-bar"></span><span class="icon-bar"></span>
                </a><a class="brand" href="#">Game Theory</a>
                <div class="nav-collapse">
                    <ul class="nav">
                        <li class="active"><a href="#">Basic Models</a></li>
                        <li><a href="#about">Comparative Advantage</a></li>
                        <li><a href="#contact">Questions of Evaluation</a></li>
                    </ul>
                </div>
                <!--/.nav-collapse -->
            </div>
        </div>
    </div>
    <div class="container">
        <section id="tables">
  <div class="page-header">
    <h1>Tables <small>For, you guessed it, tabular data</small></h1>
  </div>
  <div class="row">
      <ul class="pager">
        <li><a href="#" id="aPrevious">Previous</a></li>
        <li><a href="#" id="aNext">Next</a></li>
      </ul>
  </div>
  <table class="table table-bordered table-striped  table-condensed">
  <thead>
      <tr>
        <th>Id</th>
        <th>Name</th>
        <th>Description</th>
      </tr>
    </thead>
    <tbody id="tbList">
    <!--
      {#foreach $T.items as record begin=0}
      <tr>
        <td>{$T.record.id}</td>
        <td>{$T.record.name}</td>
        <td>{$T.record.title}</td>
      </tr>
      {#/for}
    -->  
    </tbody>
  </table>

</section>
        <hr>
        <footer>
        <p>&copy; Company 2012</p>
      </footer>
    </div>
    <!-- /container -->
    <!-- Le javascript
    ================================================== -->
    <!-- Placed at the end of the document so the pages load faster -->

    <script src="js/jquery.min.js"></script>
    <script src="js/jquery.templates.js"></script>
    <script src="js/bootstrap.min.js"></script>

    <script type="text/javascript" language="javascript">
        var data = { mod: 'uin', act: 'getlist', pagesize:20, pageindex:1 };
        function getList() {
            $.getJSON("Json.aspx", data, function(ret) {
                $("#tbList").processInnerTemplate(ret);
            });
        }
        $("#aNext").click(function(e) {
            data.pageindex++;
            getList();
            e.preventDefault();
        });
        $("#aPrevious").click(function(e) {
            if (data.pageindex > 1) {
                data.pageindex--;
                getList();
                e.preventDefault();
            }
        });
        getList();
    </script>

</body>
</html>
