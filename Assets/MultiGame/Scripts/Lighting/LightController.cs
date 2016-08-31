﻿using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[RequireComponent(typeof(Light))]
	public class LightController : MultiModule {

		private Light myLight;

		[Tooltip("A curve showing the 'Intensity' over time, zoom in & out to get larger/smaller values, hold shift/ctrl/cmd to change the zoom axis, or right-click on a node & select 'Edit' to change it's value.")]
		public AnimationCurve intensityOverTime;
		[Tooltip("Color gradient is also applied based on the length value of 'Intensity Over Time', so if the brightness total length is 3 seconds, it will take 3 seconds to go through the gradient as well.")]
		public Gradient colorOverTime;
		[Tooltip("When we reach the end, should we loop?")]
		public bool looping = false;

		private float startTime;

		public HelpInfo help = new HelpInfo("Light Controller allows you to adjust the settings of this light at run-time and animate it's brightness & color without creating a new animation");

		void Awake () {
			myLight = GetComponent<Light>();
		}

		void Start () {
			if (intensityOverTime.keys.Length < 1 ) {
				intensityOverTime.AddKey(0f,myLight.intensity);
				intensityOverTime.AddKey(1f,myLight.intensity);
			}

			startTime = Time.time;
		}
		
		void Update () {
			myLight.intensity = intensityOverTime.Evaluate((Time.time - startTime));
			myLight.color = colorOverTime.Evaluate((Time.time - startTime) / intensityOverTime.keys[intensityOverTime.length - 1].time);
			if (looping && ((Time.time - startTime) > intensityOverTime.keys[intensityOverTime.length - 1].time)) {
				startTime = Time.time;
			}
		}
	}
}