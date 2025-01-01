﻿using System.Text.Json.Serialization;

namespace RatingService;

public struct User
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }
}