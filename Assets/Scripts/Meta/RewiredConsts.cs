/* Rewired Constants
   This list was generated on 4/24/2017 11:38:24 AM
   The list applies to only the Rewired Input Manager from which it was generated.
   If you use a different Rewired Input Manager, you will have to generate a new list.
   If you make changes to the exported items in the Rewired Input Manager, you will need to regenerate this list.
*/

namespace RewiredConsts
{
	public static class Action
	{
		// Default
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Restart Scene")]
		public const int Restart_Scene = 31;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Set camera to left position")]
		public const int Camera_Left = 32;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Set camera to right position")]
		public const int Camera_Right = 33;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Set camera to behind position")]
		public const int Camera_Behind = 34;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Set camera to firstperson position")]
		public const int Camera_Firstperson = 35;
		// Pilot
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Anything movement related", friendlyName = "Move Horizontal")]
		public const int Move_Horizontal = 0;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Anything movement related", friendlyName = "Move Vertical")]
		public const int Move_Vertical = 1;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Anything movement related", friendlyName = "Turn Body Horz")]
		public const int Turn_Body_Horz = 22;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Anything movement related", friendlyName = "Turn Body Vert")]
		public const int Turn_Body_Vert = 23;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Anything movement related", friendlyName = "Crouch")]
		public const int Crouch = 25;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Anything movement related", friendlyName = "Dash")]
		public const int Dash = 26;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Anything movement related", friendlyName = "Run")]
		public const int Run = 27;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Anything movement related", friendlyName = "Kick")]
		public const int Kick = 28;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Anything movement related", friendlyName = "Lock On or Off")]
		public const int Lock_On = 36;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Anything movement related", friendlyName = "Dodge")]
		public const int Dodge = 38;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Anything movement related", friendlyName = "Jump")]
		public const int Jump = 39;
		// Arms
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Arms Actions", friendlyName = "Move left arm horionztally")]
		public const int Move_Left_Arm_X = 12;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Arms Actions", friendlyName = "Move left arm vertically")]
		public const int Move_Left_Arm_Y = 13;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Arms Actions", friendlyName = "Move right arm horionztally")]
		public const int Move_Right_Arm_X = 8;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Arms Actions", friendlyName = "Move right arm vertically")]
		public const int Move_Right_Arm_Y = 10;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Arms Actions", friendlyName = "Rotate hand around arm")]
		public const int Rotate_Hand = 11;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Arms Actions", friendlyName = "Wind Up Attack")]
		public const int Wind_Up_Attack = 18;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Arms Actions", friendlyName = "Give Pilot Energy")]
		public const int Give_Pilot_Energy = 24;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Arms Actions", friendlyName = "Take Energy From Pilot")]
		public const int Take_Energy_From_Pilot = 21;
		[Rewired.Dev.ActionIdFieldInfo(categoryName = "Arms Actions", friendlyName = "Block")]
		public const int Block = 29;
		// Engineer
		//[Rewired.Dev.ActionIdFieldInfo(categoryName = "Engineer Actions", friendlyName = "Select Elements Horizontally")]
		//public const int Select_Horizontal = 4;
		//[Rewired.Dev.ActionIdFieldInfo(categoryName = "Engineer Actions", friendlyName = "Select Elements Vertically")]
		//public const int Select_Vertical = 6;
		//[Rewired.Dev.ActionIdFieldInfo(categoryName = "Engineer Actions", friendlyName = "Select Submenu")]
		//public const int Select_Submenu = 19;
		//// Scout Drone
		//[Rewired.Dev.ActionIdFieldInfo(categoryName = "Scout Drone Actions", friendlyName = "Scout Drone Horizontal")]
		//public const int Scout_Drone_Horizontal = 15;
		//[Rewired.Dev.ActionIdFieldInfo(categoryName = "Scout Drone Actions", friendlyName = "Scout Drone Vertical")]
		//public const int Scout_Drone_Vertical = 14;
		//[Rewired.Dev.ActionIdFieldInfo(categoryName = "Scout Drone Actions", friendlyName = "Scout Drone Drive")]
		//public const int Scout_Drone_Drive = 16;
		//[Rewired.Dev.ActionIdFieldInfo(categoryName = "Scout Drone Actions", friendlyName = "Powerslide")]
		//public const int Scout_Drone_Powerslide = 17;
	}
	public static class Category
	{
		public const int Default = 0;
		public const int Pilot = 1;
		public const int Arms = 2;
		public const int Engineer = 3;
		public const int Scout_Drone = 4;
	}
	public static class Layout
	{
		public static class Joystick
		{
			public const int Default = 0;
		}
		public static class Keyboard
		{
			public const int Default = 0;
		}
		public static class Mouse
		{
			public const int Default = 0;
		}
		public static class CustomController
		{
			public const int Default = 0;
		}
	}
}
