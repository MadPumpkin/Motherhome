using System;
using UnityEngine;
using System.Collections;

namespace Legend
{
	public class Bark : Move
	{
		
	
		// Update is called once per frame
		protected override void Update()
		{
			base.Update();
			if (Enemy.Health.IsDead)
			{
				enabled = false;
				return;
			}

			if (timeToMove <= Time.time && Enemy.CurrentMove == null)
			{
				StartMove();
			}
		}

	}
}
