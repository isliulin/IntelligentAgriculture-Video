#include "Exercise.h"


CExercise::CExercise(void)
{
	System::Windows::Forms::Button^ button1 = gcnew Button;//创建按钮
	this->Controls->Add(button1);//把按钮附加到我们的窗体上
}
