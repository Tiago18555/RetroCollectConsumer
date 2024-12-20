﻿using Application.Shared;

namespace Application.Processors.RatingOperations.AddRating;

public class AddRatingResponseModel
{
    public int RatingValue { get; set; }
    public string Review { get; set; }
    public DateTime CreatedAt { get; set; }
    public InternalGame Game { get; set; }
    public InternalUser User { get; set; }
}
