<?php
// 连接数据库
$myData = mysqli_connect ( "localhost", "root", "admin" );
if (mysqli_connect_errno ()) {
	echo mysqli_connect_error ();
	return;
}
// 选择数据库
mysqli_query ( $myData, "set names utf8" );
mysqli_select_db ( $myData, "myscoresdb" );

// 查询
$sql = "select *from hiscores order by score desc limit 20";
$result = mysqli_query ( $myData, $sql ) or die ( "<br>SQL error!<br>" );
$num_result = mysqli_num_rows ( $result );
// 准备发送数据到Unity
$arr = array ();
// 将查询结果写入到Json格式的数组中
for($i = 0; $i < $num_result; $i ++) {
	$row = mysqli_fetch_array ( $result, MYSQLI_ASSOC );
	$id = $row ['id'];
	$name = $row ['name'];
	$score = $row ['score'];
	$arr [$id] ['id'] = $id;
	$arr [$id] ['username'] = $name;
	$arr [$id] ['score'] = $score;
}
mysqli_free_result ( $result );
// 关闭数据库
mysqli_close ( $myData );
// 发送Json格式的数据
echo json_encode ( $arr );