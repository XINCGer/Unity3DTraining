#include <stdlib.h>
#include <stdio.h>
#include <string.h>



#include "maibu_sdk.h"
#include "maibu_res.h"

//欢迎界面的坐标定义
#define START_SCENE_ORIGIN_X 	0
#define START_SCENE_ORIGIN_Y 	0
#define START_SCENE_SIZE_H		128
#define START_SCENE_SIZE_W 	128

//食物的初始坐标定义
#define FOOD_START_ORIGIN_X 	0
#define FOOD_START_ORIGIN_Y 	0
#define FOOD_START_SIZE_H	6
#define FOOD_START_SIZE_W 	6

//玩家的初始坐标定义
#define BOARD_START_ORIGIN_X 	64
#define BOARD_START_ORIGIN_Y 	120
#define BOARD_START_SIZE_H		5
#define BOARD_START_SIZE_W 		28

//定义玩家的移动速度
#define BOARD_MOVESPEED_L1	-2
#define BOARD_MOVESPEED_L2	-4
#define BOARD_MOVESPEED_R1	 2
#define BOARD_MOVESPEED_R2	 4

#define FOOD 0
#define BOARD 1

//欢迎场景背景图的GRect
static const GRect bmp_origin_size_bg = {
	{START_SCENE_ORIGIN_X,START_SCENE_ORIGIN_Y},
	{START_SCENE_SIZE_H,START_SCENE_SIZE_W}
};

//GRect结构体数组，用来存储Food,Player的坐标信息
static GRect bmp_origin_size[] = {
	{
		{FOOD_START_ORIGIN_X,FOOD_START_ORIGIN_Y},
		{FOOD_START_SIZE_H,FOOD_START_SIZE_W}
	},
	{
		{BOARD_START_ORIGIN_X,BOARD_START_ORIGIN_Y},
		{BOARD_START_SIZE_H,BOARD_START_SIZE_W}
	}
};
//定时器的ID
static 	uint8_t _timerID=-1;
//用来存储重力加速器的三轴信息
static  int16_t x, y, z;
/*窗口ID, 通过该窗口ID获取窗口句柄*/
static int32_t g_window_id = -1;
//当前场景的ID，通过不同的ID显示不同的场景
static 	uint8_t	sceneID =0;
//用来存储BOARD层的ID数组
static int8_t board_layer_id[2] = {-1,-1};
//用来存储FOOD层的ID数组
static int8_t food_layer_id[10] = {-1,-1};
//用来存储FOOD=的速度的数组
static int8_t food_layer_speed[10] = {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1};
//定义Player的移动速度
static  int16_t BOARD_MOVESPEED	= 0;
P_Window init_window(void);

//窗口重新载入刷新方法
static void window_reloading(void)
{
	/*根据窗口ID获取窗口句柄*/
	P_Window p_old_window = app_window_stack_get_window_by_id(g_window_id); 
	if (NULL != p_old_window)
	{
		P_Window p_window = init_window();
		if (NULL != p_window)
		{
			g_window_id = app_window_stack_replace_window(p_old_window, p_window);
		}	
	}
	
}

//根据重力感应数值获取随机数方法
uint32_t get_random_number(uint8_t min,uint8_t max)
{
	int16_t x, y, z;
	maibu_get_accel_data(&x, &y, &z);
	struct date_time datetime;
	app_service_get_datetime(&datetime);
	uint32_t ret = max - ((x+y+z+(datetime.sec*datetime.min))%(max - min+1));
	return ret;
} 

/*创建并显示图片图层，需要坐标值，得到icon_key的数组，数组需要的参数值，P_Window*/
static int32_t display_target_layer(P_Window p_window,GRect *temp_p_frame,enum GAlign how_to_align,enum GColor black_or_white,uint16_t bmp_key)
{	

	GBitmap bmp_point;
	P_Layer temp_P_Layer = NULL;

	res_get_user_bitmap(bmp_key, &bmp_point);
	LayerBitmap layer_bitmap_struct_l = {bmp_point, *temp_p_frame, how_to_align};
 	temp_P_Layer = app_layer_create_bitmap(&layer_bitmap_struct_l);
	
	if(temp_P_Layer != NULL)
	{
		app_layer_set_bg_color(temp_P_Layer, black_or_white);
		return app_window_add_layer(p_window, temp_P_Layer);
	}

	return 0;
}

//更新图片图形方法
static int32_t update_target_layer(P_Window p_window,int8_t old_id,GRect *temp_p_frame,uint16_t bmp_key)
{
	P_Layer old_layer = app_window_get_layer_by_id(p_window,old_id);
	
	GBitmap bmp_point;
	
	res_get_user_bitmap(bmp_key, &bmp_point);
	LayerBitmap layer_bitmap_struct_l = {bmp_point, *temp_p_frame, GAlignCenter};
 	P_Layer new_layer = app_layer_create_bitmap(&layer_bitmap_struct_l);
	
	if(new_layer == NULL)
	{
		return old_id;
	}	
	
	return app_window_replace_layer(p_window,old_layer,new_layer);
	
}


static void move_element(int8_t element,int16_t speed){
	P_Window p_window = app_window_stack_get_window_by_id(g_window_id);
	if(p_window != NULL)
	{	
		if(element == BOARD)
		{
			if(speed>0){
				bmp_origin_size[element].origin.x = bmp_origin_size[element].origin.x + speed;
				if(bmp_origin_size[element].origin.x>180-BOARD_START_SIZE_W){
					bmp_origin_size[element].origin.x=180-BOARD_START_SIZE_W;
				}
			}
			else{
				if(bmp_origin_size[element].origin.x<(-speed)){
					bmp_origin_size[element].origin.x=0;
				}
				else{
					bmp_origin_size[element].origin.x = bmp_origin_size[element].origin.x + speed;
				}
			}

			board_layer_id[element] = update_target_layer(p_window,board_layer_id[element],&bmp_origin_size[element],RES_BITMAP_BOARD);
			app_window_update(p_window);
		}
		else if(element == FOOD)
		{
		}
	}
}

//定时器的回调函数
void timer_callback(date_time_t tick_time, uint32_t millis, void *context){
	P_Window p_window = (P_Window)context;
	if (NULL != p_window){
		if (0 == maibu_get_accel_data(&x, &y, &z)){
			if(y>=2048+50&&y<=2048+150){
				BOARD_MOVESPEED=BOARD_MOVESPEED_L1;
			}
			else if(y>2048+150){
				BOARD_MOVESPEED=BOARD_MOVESPEED_L2;
			}
			else if(y>=2048-150&&y<=2048-50){
				BOARD_MOVESPEED=BOARD_MOVESPEED_R1;
			}
			else if(y<2048-150){
				BOARD_MOVESPEED=BOARD_MOVESPEED_R2;
			}
			move_element(BOARD,BOARD_MOVESPEED);
		}
	}
}

/*定义后退按键事件*/
static void select_back(void *context)
{
	P_Window p_window = (P_Window)context;
	if(p_window != NULL){
		sceneID=0;
		app_window_stack_pop(p_window);
	}
}

/*定义选中按键事件*/
static void select_enter(void *context)
{
	P_Window p_window = (P_Window)context;
	sceneID=1;
	window_reloading();
}

//窗口初始化函数
P_Window init_window(){
	
	P_Window p_window = app_window_create();
	if (NULL == p_window)
	{
		return NULL;
	}
	/*创建欢迎场景背景图层*/
	if(sceneID == 0)
	{
		display_target_layer(p_window,&bmp_origin_size_bg,GAlignLeft,GColorWhite,RES_BITMAP_START_SCENE);
	}
	else if(sceneID == 1)
	{
		//创建游戏场景背景图层
		display_target_layer(p_window,&bmp_origin_size_bg,GAlignLeft,GColorWhite,RES_BITMAP_GAME_SCENE_BG);
		//创建Player层
		board_layer_id[BOARD] = display_target_layer(p_window,&bmp_origin_size[BOARD],GAlignLeft,GColorWhite,RES_BITMAP_BOARD);
		food_layer_id[0] = display_target_layer(p_window,&bmp_origin_size[FOOD],GAlignLeft,GColorWhite,RES_BITMAP_BALL);
		uint8_t i=1;
		for(;i<=9;i++){
			
		}
		for(i=0;i<=9;i++){
			food_layer_speed[0]=get_random_number(2,14);
		}
		/*定义一个窗口定时器，用于监听重力加速器*/
		_timerID = app_window_timer_subscribe(p_window,10,timer_callback,(void*)p_window);
		//_timerID = app_service_timer_subscribe(10,timer_callback,(void*)p_window);	
	}
	else if(sceneID == 2){
		display_target_layer(p_window,&bmp_origin_size_bg,GAlignLeft,GColorWhite,RES_BITMAP_GAME_SCENE_BG);
	}
	app_window_click_subscribe(p_window, ButtonIdBack, select_back);
	app_window_click_subscribe(p_window, ButtonIdSelect,  select_enter);
	return p_window;

}


int main(){


	/*创建显示时间窗口*/
	P_Window p_window = init_window();
	if (p_window != NULL)
	{
		/*放入窗口栈显示*/
		g_window_id=app_window_stack_push(p_window);
	}	
	

	return 0;

}



