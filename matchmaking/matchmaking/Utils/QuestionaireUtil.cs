using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using matchmaking.Domain;

namespace matchmaking.Utils
{
    internal class QuestionaireUtil
    {
        private List<(string Question, LoverType Type)>? _shuffledQuestions;

        private readonly List<(string Question, LoverType Type)> _questions = new()
        {
            ("I feel energized after spending time with a group of people.", LoverType.SOCIAL_EXPLORER),
            ("I enjoy meeting new people frequently.", LoverType.SOCIAL_EXPLORER),
            ("I prefer social events over staying in.", LoverType.SOCIAL_EXPLORER),
            ("I easily start conversations with strangers.", LoverType.SOCIAL_EXPLORER),

            ("I spend a lot of time reflecting on my thoughts and feelings.", LoverType.DEEP_THINKER),
            ("I prefer deep conversations over small talk.", LoverType.DEEP_THINKER),
            ("I enjoy spending time alone to recharge.", LoverType.DEEP_THINKER),
            ("I often analyze my decisions carefully.", LoverType.DEEP_THINKER),

            ("I like making spontaneous plans.", LoverType.ADVENTURE_SEEKER),
            ("I enjoy trying new and unfamiliar experiences.", LoverType.ADVENTURE_SEEKER),
            ("I get bored with routine quickly.", LoverType.ADVENTURE_SEEKER),
            ("I am comfortable taking risks.", LoverType.ADVENTURE_SEEKER),

            ("I prefer having a clear plan rather than improvising.", LoverType.STABILITY_LOVER),
            ("I feel more comfortable with routines.", LoverType.STABILITY_LOVER),
            ("I like knowing what to expect in advance.", LoverType.STABILITY_LOVER),
            ("I value consistency in my daily life.", LoverType.STABILITY_LOVER),

            ("I easily understand how others are feeling.", LoverType.EMPATHETIC_CONNECTOR),
            ("I value emotional connection in relationships.", LoverType.EMPATHETIC_CONNECTOR),
            ("I am sensitive to the moods of people around me.", LoverType.EMPATHETIC_CONNECTOR),
            ("I prioritize harmony in my relationships.", LoverType.EMPATHETIC_CONNECTOR)
        };

        public QuestionaireUtil()
        {
        }

        public List<string> GetQuestions()
        {
            if (_shuffledQuestions == null)
            {
                Random random = new Random();
                _shuffledQuestions = _questions.OrderBy(q => random.Next()).ToList();
            }

            return _shuffledQuestions.Select(q => q.Question).ToList();
        }

        public void ResetShuffle()
        {
            _shuffledQuestions = null;
        }

        public LoverType CalculateLoveType(List<int> answers)
        {
            if (_shuffledQuestions == null)
                throw new InvalidOperationException("GetQuestions() must be called before CalculateLoveType().");

            Dictionary<LoverType, int> scores = new()
            {
                { LoverType.SOCIAL_EXPLORER, 0 },
                { LoverType.DEEP_THINKER, 0 },
                { LoverType.ADVENTURE_SEEKER, 0 },
                { LoverType.STABILITY_LOVER, 0 },
                { LoverType.EMPATHETIC_CONNECTOR, 0 },
            };

            for (int i = 0; i < answers.Count && i < _shuffledQuestions.Count; i++)
            {
                scores[_shuffledQuestions[i].Type] += answers[i];
            }

            int maxScore = scores.Values.Max();
            var topTypes = scores.Where(s => s.Value == maxScore).Select(s => s.Key).ToList();
            return topTypes[new Random().Next(topTypes.Count)];
        }
    }
}
