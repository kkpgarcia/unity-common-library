
using UnityEngine;
using Common.CustomTime;

/**
 * Generic class for implementing timers (specified in seconds)
 */
public class CountdownTimer
{
	private float polledTime;
	private float countdownTime;
	
	private TimeReference timeReference;

	/**
	 * Constructor with specified TimeReference.
	 */
	public CountdownTimer (float countdownTime, TimeReference timeReference)
	{
		this.timeReference = timeReference;
		
		if (countdownTime < 0) {
			Debug.LogWarning ("The specified time must be greater than zero.");
		}
		
		Reset (countdownTime); 
	}

	/**
	 * Constructor with a specified time reference name.
	 */
	public CountdownTimer (float countdownTime, string timeReferenceName) : this (countdownTime, TimeReferencePool.GetInstance ().Get (timeReferenceName))
	{
	}

	/**
	 * Constructor that uses a default TimeReference.
	 */
	public CountdownTimer (float countdownTime) : this (countdownTime, TimeReference.GetDefaultInstance ())
	{
	}

	/**
   * Updates the countdown.
   */
	public void Update ()
	{
		this.polledTime += this.timeReference.DeltaTime;
	}

	/**
     * Resets the countdown.
     */
	public void Reset ()
	{
		this.polledTime = 0;
	}

	/**
	 * Resets the countdown timer and assigns a new countdown time.
	 */
	public void Reset (float countdownTime)
	{
		Reset ();
		this.countdownTime = countdownTime;
	}

	/**
     * Returns whether or not the countdown has elapsed.
     */
	public bool HasElapsed ()
	{
		return Comparison.TolerantGreaterThanOrEquals (this.polledTime, this.countdownTime);
	}

	/**
     * Returns the ratio of polled time to countdown time.
     */
	public float GetRatio ()
	{
		float ratio = this.polledTime / this.countdownTime;
		return Mathf.Clamp (ratio, 0, 1);
	}

	/**
     * Returns the polled time since the countdown started.
     */
	public float GetPolledTime ()
	{
		return this.polledTime;
	}

	/**
	 * Sets the polled time
	 */
	public void SetPolledTime (float polledTime)
	{
		this.polledTime = polledTime;
	}

	/**
     * Forces the timer to end.
     */
	public void EndTimer ()
	{
		this.polledTime = this.countdownTime;
	}

	/**
     * Adjusts the countdownTime
     */
	public void SetCountdownTime (float newTime)
	{
		this.countdownTime = newTime;
	}

	/**
     * Gets the countdownTime
     */
	public float GetCountdownTime ()
	{
		return this.countdownTime;
	}

	/**
	 * Gets the countdown time as a string.
	 */
	public string GetCountdownTimeString ()
	{
		float timeRemaining = countdownTime - polledTime;
		int minutes = (int)(timeRemaining / 60.0f);
		int seconds = ((int)timeRemaining) % 60;
		return string.Format ("{0}:{1:d2}", minutes, seconds);
	}
}
