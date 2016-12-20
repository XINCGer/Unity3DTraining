#include <stdlib.h>
#include <stdio.h>
#include <string.h>



#include "maibu_sdk.h"
#include "maibu_res.h"


/*显示重力传感器文本图层*/
#define SYSINFO_ACCEL_ORIGIN_X		20
#define SYSINFO_ACCEL_ORIGIN_Y		40
#define SYSINFO_ACCEL_SIZE_H		16		
#define SYSINFO_ACCEL_SIZE_W		108

	
static 	uint8_t _timerID=0;
static  uint8_t _layerId=0;
static  char str[20] = "";
static  int16_t x, y, z;


void timer_callback(date_time_t tick_time, uint32_t millis, void *context){
	P_Window p_window = (P_Window)context;
	if (NULL != p_window){
		/*根据图层ID获取图层句柄*/
		P_Layer p_layer = app_window_get_layer_by_id(p_window, _layerId);
		if (0 == maibu_get_accel_data(&x, &y, &z)){
		sprintf(str, "x轴: %d", x);
		/*添加重力传感器数据图层*/
	    GRect frame_accel = {{SYSINFO_ACCEL_ORIGIN_X, SYSINFO_ACCEL_ORIGIN_Y}, {SYSINFO_ACCEL_SIZE_H, SYSINFO_ACCEL_SIZE_W}};
		LayerText lt_accel = {str, frame_accel, GAlignLeft, U_ASCII_ARIAL_12, 0};
			if(p_layer != NULL){
				 app_layer_set_text_text(p_layer, str);
			}
		}
	}
}

P_Window init_sysinfo_window(){
	P_Window p_window = app_window_create();
	if (NULL == p_window)
	{
		return NULL;
	}
	/*添加重力传感器数据图层*/
	GRect frame_accel = {{SYSINFO_ACCEL_ORIGIN_X, SYSINFO_ACCEL_ORIGIN_Y}, {SYSINFO_ACCEL_SIZE_H, SYSINFO_ACCEL_SIZE_W}};
	
 	if (0 == maibu_get_accel_data(&x, &y, &z)){
		sprintf(str, "x轴: %d", x);
		LayerText lt_accel = {str, frame_accel, GAlignLeft, U_ASCII_ARIAL_12, 0};
		P_Layer layer_text_accel = app_layer_create_text(&lt_accel);
		if(layer_text_accel != NULL){
			_layerId = app_window_add_layer(p_window, layer_text_accel);
		}
	}
	_timerID = app_service_timer_subscribe(10,timer_callback,(void*)p_window);
	return p_window;

}




int main(){


	/*创建显示时间窗口*/
	P_Window p_window = init_sysinfo_window();
	if (p_window != NULL)
	{
		/*放入窗口栈显示*/
		app_window_stack_push(p_window);
	}	
	

	return 0;

}



