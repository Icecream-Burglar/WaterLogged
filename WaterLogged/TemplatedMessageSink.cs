﻿using System;
using System.Collections.Generic;
using System.Text;
using WaterLogged.Templating;

namespace WaterLogged
{
    /// <summary>
    /// Base class for TemplatedMessageSink implementations.
    /// </summary>
    public abstract class TemplatedMessageSink
    {
        /// <summary>
        /// Gets or sets a value indicating if this <see cref="TemplatedMessageSink"/> implementation is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a filter that filters messages that will be output through this listener.
        /// </summary>
        public FilterManager FilterManager
        {
            get => _filterManager;
            set => _filterManager = value ?? throw new ArgumentNullException(nameof(FilterManager), "You cannot have a null filter.");
        }

        /// <summary>
        /// Gets the <see cref="Log"/> that owns this TemplatedMessageSink.
        /// </summary>
        public Log Log { get; internal set; }
        
        private FilterManager _filterManager;

        protected TemplatedMessageSink()
        {
            Enabled = true;
            FilterManager = new FilterManager();
        }

        /// <summary>
        /// When overridden in a derived class; Handles an output message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">The message's tag.</param>
        public abstract void ProcessMessage(StructuredMessage message, string tag);
    }
}
