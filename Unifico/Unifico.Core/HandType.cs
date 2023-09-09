﻿using Unifico.Core.Hand;

namespace Unifico.Core;

public enum HandType
{
    ListHand
}

public static class HandTypeExtensions
{
    public static IHand Create(this HandType handType)
    {
        return handType switch
        {
            HandType.ListHand => new ListHand(),
            _ => throw new ArgumentOutOfRangeException(nameof(handType), handType, null)
        };
    }
}