﻿using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCards.Models
{
    public class CardSubject
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey(nameof(CardCategory))]
        public int CardCategoryId { get; set; }
        public CardCategory CardCategory { get; set; }

        public List<CardSet> CardSets { get; set; }
    }
}
