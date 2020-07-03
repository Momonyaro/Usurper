using QFSW.QC;
using UnityEngine;

// First investigate if this can be implemented this way!!!

// The idea is that by incrementing a in every time a turn ends, we can divide that int into 24 segments.
// These segments is for the 24 hours in a day. We want a sin wave-like function where there is a smooth
// increase and decrease of light over the span of the 24 hours. Check how to do this.

// If the above can be implemented, we can make the MapLighter set it's ambientLightLevel to the hour's x/24 percentage
// If a smoother light transition is desired, we can always just get the percentage of the total turn counts instead...

namespace RULESET.WORLD
{
	public delegate void OnClockUpdate();

	public class Clock
	{
		public static OnClockUpdate onClockUpdate;
		public static Clock instance;
		//We need hours, ticks, day, week, month stored

		public static int currentTick =   0, totalTicks = 100;	// totalTicks  = 1 hour
		public static int currentHour =  11, totalHours = 24;	// totalHours  = 1 day
		public static int currentDay =    0, totalDays = 7;		// totalDays   = 1 Week
		public static int currentWeek =   2, totalWeeks = 4;	// totalWeeks  = 1 Month
		public static int currentMonth =  0, totalMonths = 12;	// totalMonths = 1 Year
		public static int currentYear = 253;


		[Command()]
		public static void PrintCalendar()
		{
			Debug.Log("T: " + currentTick + "/" + totalTicks + " H: " + currentHour + "/" + totalHours + " D: " + currentDay + "/" + totalDays +
					 " W: " + currentWeek + "/" + totalWeeks + " M: " + currentMonth + "/" + totalMonths + " Y: " + currentYear);
		}

		[Command()]
		public static void AdvanceTime(int amount)
		{
			currentTick += amount;
			while (currentTick >= totalTicks)
			{
				currentTick -= totalTicks;
				currentHour++;
			}

			//onClockUpdate.Invoke();
			if (currentHour >= totalHours) Clock.instance.IncrementHoursByAmount(0);
		}

		public void IncrementTicksByAmount(int amount)
		{
			currentTick += amount;
			while (currentTick >= totalTicks)
			{
				currentTick -= totalTicks;
				currentHour++;
			}

			//onClockUpdate.Invoke();
			if (currentHour >= totalHours) IncrementHoursByAmount(0);
		}

		public void IncrementHoursByAmount(int amount)
		{
			currentHour += amount;
			while (currentHour >= totalHours)
			{
				currentHour -= totalHours;
				currentDay++;
			}

			//onClockUpdate.Invoke();
			if (currentDay >= totalDays) IncrementDaysByAmount(0);
		}

		public void IncrementDaysByAmount(int amount)
		{
			currentDay += amount;
			while (currentDay >= totalDays)
			{
				currentDay -= totalDays;
				currentWeek++;
			}

			//onClockUpdate.Invoke();
			if (currentWeek >= totalWeeks) IncrementWeeksByAmount(0);
		}

		public void IncrementWeeksByAmount(int amount)
		{
			currentWeek += amount;
			while (currentWeek >= totalWeeks)
			{
				currentWeek -= totalWeeks;
				currentMonth++;
			}

			//onClockUpdate.Invoke();
			if (currentMonth >= totalMonths) IncrementMonthsByAmount(0);
		}

		public void IncrementMonthsByAmount(int amount)
		{
			currentMonth += amount;
			while (currentMonth >= totalMonths)
			{
				currentMonth -= totalMonths;
				currentYear++;
			}

			//onClockUpdate.Invoke();
		}

		public void IncrementYearByAmount (int amount)
		{
			currentYear += amount;
			//onClockUpdate.Invoke();
		}

		// Constructing a almenac. Should we expose it in the editor or do we just want it constant.
		// Until it's been decided, I guess we can ignore everything but the hours and ticks
	}
}