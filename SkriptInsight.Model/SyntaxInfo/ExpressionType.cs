using System;

namespace SkriptInsight.Model.SyntaxInfo
{
    public enum ExpressionType
    {
        /// <summary>
        /// Expressions that only match simple text, e.g. <code>"[the] player"</code>
        /// </summary>
        Simple,
        /// <summary>
        /// I don't know what this was used for. It will be removed or renamed in the future.
        /// </summary>
        [Obsolete("Please use Simple instead of Normal")]
        Normal,

        /// <summary>
        /// Expressions that contain other expressions, e.g. <code>"[the] distance between %location% and %location%"</code>
        /// <p/>
        /// See <see cref="Property"/>
        /// </summary>
        Combined,
        /// <summary>
        /// Property expressions, e.g. <code>"[the] data value[s] of %items%"/"%items%'[s] data value[s]"</code>
        /// </summary>
        Property,
        /// <summary>
        /// Expressions whose pattern matches (almost) everything, e.g. <code>"[the] [event-]\&lt;.+&gt;"</code>
        /// </summary>
        PatternMatchesEverything
    }
}