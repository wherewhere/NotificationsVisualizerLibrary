using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NotificationsVisualizerLibrary
{
    /// <summary>
    /// Provides polyfills for missing .NET features.
    /// </summary>
    internal static class Polyfills
    {
#if !NET
        /// <summary>
        /// The extension for the <see cref="Enum"/> class.
        /// </summary>
        extension(Enum)
        {
            /// <summary>
            /// Retrieves an array of the values of the constants in a specified enumeration type.
            /// </summary>
            /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
            /// <returns>An array that contains the values of the constants in <typeparamref name="TEnum"/>.</returns>
            public static TEnum[] GetValues<TEnum>() where TEnum : struct, Enum => [.. Enum.GetValues(typeof(TEnum)).OfType<TEnum>()];
        }
#endif

#if !NET6_0_OR_GREATER
        /// <summary>
        /// The extension for the <see cref="ArgumentNullException"/> class.
        /// </summary>
        extension(ArgumentNullException)
        {
            /// <summary>
            /// Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.
            /// </summary>
            /// <param name="argument">The reference type argument to validate as non-null.</param>
            /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
            public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
            {
                if (argument is null)
                {
                    throw new ArgumentNullException(paramName);
                }
            }
        }
#endif

#if !COMP_NETSTANDARD2_1
        /// <summary>
        /// The extension for the <see cref="string"/> class.
        /// </summary>
        /// <param name="text">The <see cref="string"/> to extend.</param>
        extension(string text)
        {
            /// <summary>
            /// Determines whether the end of this string instance matches the specified character.
            /// </summary>
            /// <param name="value">The character to compare to the character at the end of this instance.</param>
            /// <returns><see langword="true"/> if <paramref name="value"/> matches the end of this instance; otherwise, <see langword="false"/>.</returns>
            public bool EndsWith(char value) => text.EndsWith(new string([value]));

            /// <summary>
            /// Determines whether this string instance starts with the specified character.
            /// </summary>
            /// <param name="value">The character to compare.</param>
            /// <returns><see langword="true"/> if <paramref name="value"/> matches the beginning of this instance; otherwise, <see langword="false"/>.</returns>
            public bool StartsWith(char value) => text.StartsWith(new string([value]));
        }
#endif
    }
}
