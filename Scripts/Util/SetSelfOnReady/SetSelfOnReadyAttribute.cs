using System;

/// <summary>
/// An attribute marking a property to be set to itself on _Ready.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = true)]
public sealed class SetSelfOnReadyAttribute : Attribute { }

