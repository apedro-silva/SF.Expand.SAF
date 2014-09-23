using System;
namespace SF.Expand.SAF.Defs
{
	public class TokenTimeFactory
	{
		private int iTimeFactor;
		private int iInitialClockDrift;
		private long lStartTime;
		private long lNegativeRange;
		private long lPositiveRange;
		private long lProgressiveClock;
		public TokenTimeFactory(int iClockDrift, long lLastAuth, int iRange, int iTimeFactor)
		{
			this.iTimeFactor = iTimeFactor;
			this.iInitialClockDrift = iClockDrift;
			this.lStartTime = (long)iClockDrift + (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds / (long)iTimeFactor;
			this.lNegativeRange = this.lStartTime - (long)iRange;
			this.lPositiveRange = this.lStartTime + (long)iRange;
			if (this.lNegativeRange <= lLastAuth)
			{
				this.lNegativeRange = lLastAuth + 1L;
			}
			this.lProgressiveClock = this.lNegativeRange - 1L;
		}
		public bool onTime()
		{
			return this.lProgressiveClock < this.lPositiveRange;
		}
		public int getClockDrift()
		{
			return (int)(this.lProgressiveClock - (this.lStartTime - (long)this.iInitialClockDrift));
		}
		public string getCurrentClock()
		{
			DateTime dateTime = new DateTime(1970, 1, 1);
			return dateTime.AddSeconds((double)(this.lProgressiveClock * (long)this.iTimeFactor)).ToString("ddd MMM dd hh:mm:ss \"GMT\" yyyy");
		}
		public int getPrecision()
		{
			return (int)(this.lProgressiveClock - this.lStartTime);
		}
		public long getTimePosition()
		{
			return this.lProgressiveClock;
		}
	}
}
