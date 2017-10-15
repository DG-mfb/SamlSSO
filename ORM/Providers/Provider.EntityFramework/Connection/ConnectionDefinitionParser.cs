﻿namespace Provider.EntityFramework.Connection
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reflection;
    using Kernel.Data.Connection;
    using Kernel.Initialisation;

    /// <summary>
    /// Parses a configuration section and builds an IDbConnectionDefinition.
    /// </summary>
    internal class ConnectionDefinitionParser : IConnectionDefinitionParser, IAutoRegisterAsTransient
	{
		#region fields

		private readonly Func<NameValueCollection> ConnectionPropertiesFactory;
		private IDbConnectionDefinition definition;
		private readonly Func<PropertyInfo, string> configNameConverter;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of connection definition parser
		/// </summary>
		/// <param name="connectionPropertiesFactory"></param>
		public ConnectionDefinitionParser(Func<NameValueCollection> connectionPropertiesFactory, Func<PropertyInfo, string> configNameConverter)
		{
			this.ConnectionPropertiesFactory = connectionPropertiesFactory;
			this.configNameConverter = configNameConverter;
		}

		#endregion

		#region Methods

		/// <summary>
		///     Gets Connection definition
		/// </summary>
		public IDbConnectionDefinition ConnectionDefinition
		{
			get
			{
				if (this.definition == null)
					this.definition = this.ParseConnectionDefinition();

				return this.definition;
			}
		}

		/// <summary>
		///     Parses the connection definition from NameValueCollection dynamically by reflection.
		/// </summary>
		/// <param name="appSettings">The application settings.</param>
		/// <returns>IDbConnectionDefinition.</returns>
		/// <exception cref="System.ArgumentNullException">settings</exception>
		protected IDbConnectionDefinition ParseConnectionDefinition()
		{
			if (this.ConnectionPropertiesFactory == null)
				throw new ArgumentNullException("settings");

			var nameValueCollection = this.ConnectionPropertiesFactory();
			var nameValueDictionary = nameValueCollection.AllKeys.ToDictionary(k => k, v => nameValueCollection[v]);

			var properties = typeof(DbConnectionDefinition).GetProperties();
			var jouned = properties
				.Join
				(
					nameValueDictionary,
					k1 => this.configNameConverter(k1),
					k2 => k2.Key,
					(propertyInfo, kvp) => new
					{
						propertyInfo,
						value = kvp.Value
					},
					StringComparer.OrdinalIgnoreCase
				);

			var result = jouned
				.Aggregate(new DbConnectionDefinition(), (definition, item) =>
				{
					item.propertyInfo.SetValue(definition, item.value);
					return definition;
				});

			return result;
		}

		#endregion
	}
}