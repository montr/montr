﻿using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Montr.Idx.Commands
{
	public class ExternalLogin : IRequest<ChallengeResult>
	{
		public string ReturnUrl { get; set; }

		[Required]
		[StringLength(128)]
		public string Provider { get; set; }
	}
}
