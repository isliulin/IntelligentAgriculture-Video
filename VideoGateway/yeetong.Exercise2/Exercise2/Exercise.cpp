#include "Exercise.h"


CExercise::CExercise(void)
{
	System::Windows::Forms::Button^ button1 = gcnew Button;//������ť
	this->Controls->Add(button1);//�Ѱ�ť���ӵ����ǵĴ�����
}
