﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaterLogged.Templating;

namespace WaterLogged
{
    /// <summary>
    /// Represents a Log.
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Gets the name of this log.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Gets or sets a value indicating if this <see cref="Log"/> is enabled.
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Gets or sets the formatter to pass messages through.
        /// </summary>
        public Formatter Formatter { get; set; }
        /// <summary>
        /// Gets an array of <see cref="Listeners"/>.
        /// </summary>
        public Listener[] Listeners { get { return _listeners.Values.ToArray(); } }
        /// <summary>
        /// Gets an array of <see cref="TemplatedMessageSink"/>.
        /// </summary>
        public TemplatedMessageSink[] Sinks { get { return _sinks.Values.ToArray(); } }

        /// <summary>
        /// Gets or sets a filter to use for all logging messages.
        /// </summary>
        public FilterManager Filter { get; set; }

        /// <summary>
        /// Gets or sets a tag to use when logging messages and no tag is specified.
        /// </summary>
        public string DefaultTag { get; set; }

        private Dictionary<string, Listener> _listeners;
        private Dictionary<string, TemplatedMessageSink> _sinks;

        /// <summary>
        /// Instantiates an instance of <see cref="Log"/> with a default name.
        /// </summary>
        public Log()
            : this(string.Format("log{0}", DateTime.Now.Ticks))
        {
        }

        /// <summary>
        /// Instantiates an instance of <see cref="Log"/> with the specified name.
        /// </summary>
        /// <param name="name">The name of the Log.</param>
        public Log(string name)
        {
            _listeners = new Dictionary<string, Listener>();
            _sinks = new Dictionary<string, TemplatedMessageSink>();

            Name = name;
            Filter = new FilterManager();
            Enabled = true;
            DefaultTag = "";

            Global.LogCreated(this);
        }

        /// <summary>
        /// Adds a <see cref="Listener"/> to this Log.
        /// </summary>
        /// <param name="listener">The Listener to add.</param>
        public Log AddListener(Listener listener)
        {
            if (listener.Log != null)
            {
                throw new InvalidOperationException("A listener may only be bound to one log at a time.");
            }

            if (string.IsNullOrWhiteSpace(listener.Name))
            {
                listener.Name = DateTime.Now.Ticks.ToString();
            }
            listener.Log = this;
            _listeners.Add(listener.Name, listener);
            return this;
        }

        /// <summary>
        /// Returns a value indicating if this Log contains a <see cref="Listener"/> with the specified name.
        /// </summary>
        /// <param name="name">The name of the Listener to search for.</param>
        /// <returns></returns>
        public bool ContainsListener(string name)
        {
            return _listeners.ContainsKey(name);
        }

        /// <summary>
        /// Finds a <see cref="Listener"/> with the specified name and returns it.
        /// </summary>
        /// <param name="name">The name of the Listener.</param>
        public Listener GetListener(string name)
        {
            return _listeners[name];
        }

        /// <summary>
        /// Removes the <see cref="Listener"/> with the specified name from this Log.
        /// </summary>
        /// <param name="name">The name of the Listener to remove.</param>
        public Log RemoveListener(string name)
        {
            _listeners[name].Log = null;
            _listeners.Remove(name);
            return this;
        }

        /// <summary>
        /// Changes a <see cref="Listener"/>'s name.
        /// </summary>
        /// <param name="oldName">The name of the Listener whose name to change.</param>
        /// <param name="newName">The new name of the Listener.</param>
        public Log ChangeListenerName(string oldName, string newName)
        {
            var oldListener = _listeners[oldName];
            _listeners.Add(newName, oldListener);
            _listeners.Remove(oldName);
            return this;
        }


        /// <summary>
        /// Adds a <see cref="TemplatedMessageSink"/> to this Log.
        /// </summary>
        /// <param name="sink">The sink to add.</param>
        public Log AddSink(TemplatedMessageSink sink)
        {
            if (sink.Log != null)
            {
                throw new InvalidOperationException("A template message sink may only be bound to one log at a time.");
            }

            if (string.IsNullOrWhiteSpace(sink.Name))
            {
                sink.Name = DateTime.Now.Ticks.ToString();
            }
            sink.Log = this;
            _sinks.Add(sink.Name, sink);
            return this;
        }

        /// <summary>
        /// Returns a value indicating if this Log contains a <see cref="TemplatedMessageSink"/> with the specified name.
        /// </summary>
        /// <param name="name">The name of the TemplatedMessageSink to search for.</param>
        /// <returns></returns>
        public bool ContainsSink(string name)
        {
            return _sinks.ContainsKey(name);
        }

        /// <summary>
        /// Finds a <see cref="TemplatedMessageSink"/> with the specified name and returns it.
        /// </summary>
        /// <param name="name">The name of the TemplatedMessageSink.</param>
        public TemplatedMessageSink GetSink(string name)
        {
            return _sinks[name];
        }

        /// <summary>
        /// Removes the <see cref="TemplatedMessageSink"/> with the specified name from this Log.
        /// </summary>
        /// <param name="name">The name of the TemplatedMessageSink to remove.</param>
        public Log RemoveSink(string name)
        {
            _sinks[name].Log = null;
            _sinks.Remove(name);
            return this;
        }

        /// <summary>
        /// Changes a <see cref="TemplatedMessageSink"/>'s name.
        /// </summary>
        /// <param name="oldName">The name of the TemplatedMessageSink whose name to change.</param>
        /// <param name="newName">The new name of the TemplatedMessageSink.</param>
        public Log ChangeSinkName(string oldName, string newName)
        {
            var oldSink = _sinks[oldName];
            _sinks.Add(newName, oldSink);
            _sinks.Remove(oldName);
            return this;
        }

        //********************************************
        // WriteLine
        //********************************************

        /// <summary>
        /// Outputs a message (first passing it through string.Format) followed by a line terminator.
        /// </summary>
        /// <param name="value">The message to write.</param>
        /// <param name="arg0">A string.Format argument.</param>
        public void WriteLine(string value, object arg0)
        {
            WriteLine(string.Format(value, arg0));
        }

        /// <summary>
        /// Outputs a message (first passing it through string.Format) followed by a line terminator.
        /// </summary>
        /// <param name="value">The message to write.</param>
        /// <param name="arg0">A string.Format argument.</param>
        /// <param name="arg1">A string.Format argument.</param>
        public void WriteLine(string value, object arg0, object arg1)
        {
            WriteLine(string.Format(value, arg0, arg1));
        }

        /// <summary>
        /// Outputs a message (first passing it through string.Format) followed by a line terminator.
        /// </summary>
        /// <param name="value">The message to write.</param>
        /// <param name="arg0">A string.Format argument.</param>
        /// <param name="arg1">A string.Format argument.</param>
        /// <param name="arg2">A string.Format argument.</param>
        public void WriteLine(string value, object arg0, object arg1, object arg2)
        {
            WriteLine(string.Format(value, arg0, arg1, arg2));
        }

        /// <summary>
        /// Outputs a message (first passing it through string.Format) followed by a line terminator.
        /// </summary>
        /// <param name="value">The message to write.</param>
        /// <param name="args">An array of string.Format arguments.</param>
        public void WriteLine(string value, params object[] args)
        {
            WriteLine(string.Format(value, args));
        }

        /// <summary>
        /// Outputs a message followed by a line terminator.
        /// </summary>
        /// <param name="value">The message to write.</param>
        public void WriteLine(string value)
        {
            WriteTag(value + Environment.NewLine, "");
        }


        //********************************************
        // Write
        //********************************************

        /// <summary>
        /// Outputs a message (first passing it through string.Format).
        /// </summary>
        /// <param name="value">The message to write.</param>
        /// <param name="arg0">A string.Format argument.</param>
        public void Write(string value, object arg0)
        {
            Write(string.Format(value, arg0));
        }

        /// <summary>
        /// Outputs a message (first passing it through string.Format).
        /// </summary>
        /// <param name="value">The message to write.</param>
        /// <param name="arg0">A string.Format argument.</param>
        /// <param name="arg1">A string.Format argument.</param>
        public void Write(string value, object arg0, object arg1)
        {
            Write(string.Format(value, arg0, arg1));
        }

        /// <summary>
        /// Outputs a message (first passing it through string.Format).
        /// </summary>
        /// <param name="value">The message to write.</param>
        /// <param name="arg0">A string.Format argument.</param>
        /// <param name="arg1">A string.Format argument.</param>
        /// <param name="arg2">A string.Format argument.</param>
        public void Write(string value, object arg0, object arg1, object arg2)
        {
            Write(string.Format(value, arg0, arg1, arg2));
        }

        /// <summary>
        /// Outputs a message (first passing it through string.Format).
        /// </summary>
        /// <param name="value">The message to write.</param>
        /// <param name="args">An array of string.Format arguments.</param>
        public void Write(string value, params object[] args)
        {
            Write(string.Format(value, args));
        }

        /// <summary>
        /// Outputs a message.
        /// </summary>
        /// <param name="value">The message to write.</param>
        public void Write(string value)
        {
            WriteTag(value, "");
        }


        //********************************************
        // WriteLineTag
        //********************************************

        /// <summary>
        /// Outputs a message (first passing it through string.Format) followed by a line terminator using the specified tag.
        /// </summary>
        /// <param name="value">The message to write.</param>
        /// <param name="tag">The message's tag.</param>
        /// <param name="arg0">A string.Format argument.</param>
        public void WriteLineTag(string value, string tag, object arg0)
        {
            WriteLineTag(string.Format(value, arg0), tag);
        }

        /// <summary>
        /// Outputs a message (first passing it through string.Format) followed by a line terminator using the specified tag.
        /// </summary>
        /// <param name="value">The message to write.</param>
        /// <param name="tag">The message's tag.</param>
        /// <param name="arg0">A string.Format argument.</param>
        /// <param name="arg1">A string.Format argument.</param>
        public void WriteLineTag(string value, string tag, object arg0, object arg1)
        {
            WriteLineTag(string.Format(value, arg0, arg1), tag);
        }

        /// <summary>
        /// Outputs a message (first passing it through string.Format) followed by a line terminator using the specified tag.
        /// </summary>
        /// <param name="value">The message to write.</param>
        /// <param name="tag">The message's tag.</param>
        /// <param name="arg0">A string.Format argument.</param>
        /// <param name="arg1">A string.Format argument.</param>
        /// <param name="arg2">A string.Format argument.</param>
        public void WriteLineTag(string value, string tag, object arg0, object arg1, object arg2)
        {
            WriteLineTag(string.Format(value, arg0, arg1, arg2), tag);
        }

        /// <summary>
        /// Outputs a message (first passing it through string.Format) followed by a line terminator using the specified tag.
        /// </summary>
        /// <param name="value">The message to write.</param>
        /// <param name="tag">The message's tag.</param>
        /// <param name="args">string.Format arguments.</param>
        public void WriteLineTag(string value, string tag, params object[] args)
        {
            WriteLineTag(string.Format(value, args), tag);
        }

        /// <summary>
        /// Outputs a message followed by a line terminator using the specified tag.
        /// </summary>
        /// <param name="value">The message to write.</param>
        /// <param name="tag">The message's tag.</param>
        public void WriteLineTag(string value, string tag)
        {
            WriteTag(value + Environment.NewLine, tag);
        }


        //********************************************
        // WriteTag
        //********************************************

        /// <summary>
        /// Outputs a message (first passing it through string.Format) using the specified tag.
        /// </summary>
        /// <param name="value">The message to write.</param>
        /// <param name="tag">The message's tag.</param>
        /// <param name="arg0">A string.Format argument.</param>
        public void WriteTag(string value, string tag, object arg0)
        {
            WriteTag(string.Format(value, arg0), tag);
        }

        /// <summary>
        /// Outputs a message (first passing it through string.Format) using the specified tag.
        /// </summary>
        /// <param name="value">The message to write.</param>
        /// <param name="tag">The message's tag.</param>
        /// <param name="arg0">A string.Format argument.</param>
        /// <param name="arg1">A string.Format argument.</param>
        public void WriteTag(string value, string tag, object arg0, object arg1)
        {
            WriteTag(string.Format(value, arg0, arg1), tag);
        }

        /// <summary>
        /// Outputs a message (first passing it through string.Format) using the specified tag.
        /// </summary>
        /// <param name="value">The message to write.</param>
        /// <param name="tag">The message's tag.</param>
        /// <param name="arg0">A string.Format argument.</param>
        /// <param name="arg1">A string.Format argument.</param>
        /// <param name="arg2">A string.Format argument.</param>
        public void WriteTag(string value, string tag, object arg0, object arg1, object arg2)
        {
            WriteTag(string.Format(value, arg0, arg1, arg2), tag);
        }

        /// <summary>
        /// Outputs a message (first passing it through string.Format) using the specified tag.
        /// </summary>
        /// <param name="value">The message to write.</param>
        /// <param name="tag">The message's tag.</param>
        /// <param name="args">string.Format arguments.</param>
        public void WriteTag(string value, string tag, params object[] args)
        {
            WriteTag(string.Format(value, args), tag);
        }

        /// <summary>
        /// Outputs a message followed using the specified tag.
        /// </summary>
        /// <param name="value">The message to write.</param>
        /// <param name="tag">The message's tag.</param>
        public void WriteTag(string value, string tag)
        {
            if (!Enabled)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(tag))
            {
                tag = DefaultTag;
            }

            string formattedMessage = value;
            if (Formatter != null)
            {
                formattedMessage = Formatter.Transform(this, value, tag, new Dictionary<string, string>());
            }

            if (!Filter.Validate(value, tag))
            {
                return;
            }

            if(LogPool.StageMessage(this, formattedMessage, tag))
            {
                return;
            }

            PushMessage(formattedMessage, tag);
        }


        //********************************************
        // WriteObject
        //********************************************

        /// <summary>
        /// Outputs the specified object as a message. Transforming the object to a friendly-string if possible.
        /// </summary>
        /// <param name="value">The input object.</param>
        /// <param name="argument">An optional argument to pass to an <see cref="ObjectTransformer"/>.</param>
        /// <remarks>
        /// If <see cref="WaterLogged.ObjectTransformer"/> has a Transformer set to the input object's type,
        /// the input object will be transformed into a friendly string through the Transformer.
        /// If a Transformer can't be resolved in this way; the input object's ToString() will be used.
        /// </remarks>
        public void WriteObject(object value, object argument = null)
        {
            WriteObjectTag(value, "", argument);
        }

        /// <summary>
        /// Outputs the specified object as a message. Using the specified
        /// <see cref="ObjectTransformer"/> to transform the object to a friendly-string.
        /// </summary>
        /// <param name="value">The input object.</param>
        /// <param name="transformer">The <see cref="ObjectTransformer"/> to use to transform
        /// the input object into a friendly string.</param>
        /// <param name="argument">An optional argument to pass to the <see cref="ObjectTransformer"/>.</param>
        public void WriteObject(object value, ObjectTransformer transformer, object argument = null)
        {
            WriteObjectTag(value, "", transformer, argument);
        }


        //********************************************
        // WriteObjectTag
        //********************************************

        /// <summary>
        /// Outputs the specified object as a message with the specified tag. Transforming the object to a friendly-string if possible.
        /// </summary>
        /// <param name="value">The input object.</param>
        /// <param name="tag">The message's tag.</param>
        /// <param name="argument">An optional argument to pass to an <see cref="ObjectTransformer"/>.</param>
        /// <remarks>
        /// If <see cref="WaterLogged.ObjectTransformer"/> has a Transformer set to the input object's type,
        /// the input object will be transformed into a friendly string through the Transformer.
        /// If a Transformer can't be resolved in this way; the input object's ToString() will be used.
        /// </remarks>
        public void WriteObjectTag(object value, string tag, object argument = null)
        {
            string result = "";
            if (ObjectTransformer.ContainsTransformer(value.GetType()))
            {
                result = ObjectTransformer.GetTransformer(value.GetType()).Transform(value, argument);
            }
            else
            {
                result = value.ToString();
            }
            WriteTag(result, tag);
        }

        /// <summary>
        /// Outputs the specified object as a message with the specified tag. Using the specified
        /// <see cref="ObjectTransformer"/> to transform the object to a friendly-string.
        /// </summary>
        /// <param name="value">The input object.</param>
        /// <param name="tag">The message's tag.</param>
        /// <param name="transformer">The <see cref="ObjectTransformer"/> to use to transform
        /// the input object into a friendly string.</param>
        /// <param name="argument">An optional argument to pass to the <see cref="ObjectTransformer"/>.</param>
        public void WriteObjectTag(object value, string tag, ObjectTransformer transformer, object argument = null)
        {
            WriteTag(transformer.Transform(value, argument), tag);
        }
        

        //********************************************
        // WriteStructured
        //********************************************

        /// <summary>
        /// Outputs a structured message matching the hole names with the first item of each <see cref="Tuple"/> in an array of hole values.
        /// </summary>
        /// <param name="template">The template string.</param>
        /// <param name="tag">The message's tag.</param>
        /// <param name="holeValues">An array of <see cref="Tuple"/> with a length of 2 with the first item
        /// correlating to a hole name and the second item holding the actual hole value.</param>
        public void WriteStructuredNamed(string template, string tag, params (string, object)[] holeValues)
        {
            if (!Enabled)
            {
                return;
            }
            if (Formatter != null)
            {
                template = Formatter.Transform(template, this, tag, new Dictionary<string, string>());
            }
            var message = TemplateProcessor.BuildNamedMessage(template, holeValues);
            if(LogPool.StageMessage(this, message, tag))
            {
                return;
            }
            WriteStructuredMessage(message, tag);
        }

        /// <summary>
        /// Outputs a structured message using an array of hole values.
        /// </summary>
        /// <param name="template">The template string.</param>
        /// <param name="tag">The message's tag.</param>
        /// <param name="holeValues">An array of hole values.</param>
        public void WriteStructured(string template, string tag, params object[] holeValues)
        {
            if (!Enabled)
            {
                return;
            }
            if (Formatter != null)
            {
                template = Formatter.Transform(template, this, tag, new Dictionary<string, string>());
            }
            var message = TemplateProcessor.BuildMessage(template, holeValues);
            if(LogPool.StageMessage(this, message, tag))
            {
                return;
            }
            WriteStructuredMessage(message, tag);
        }

        /// <summary>
        /// Outputs a structured message matching the holes with properties and fields in the specified <paramref name="parentObject"/>.
        /// </summary>
        /// <param name="template">The template string.</param>
        /// <param name="tag">The message's tag.</param>
        /// <param name="parentObject">The parent object that is host to hole values.</param>
        public void WriteStructuredParent(string template, string tag, object parentObject)
        {
            if (!Enabled)
            {
                return;
            }
            if (Formatter != null)
            {
                template = Formatter.Transform(template, this, tag, new Dictionary<string, string>());
            }
            var message = TemplateProcessor.BuildParentMessage(template, parentObject);
            if(LogPool.StageMessage(this, message, tag))
            {
                return;
            }
            WriteStructuredMessage(message, tag);
        }

        /// <summary>
        /// Outputs a structured message matching the holes with properties and fields in the specified static type.
        /// </summary>
        /// <param name="template">The template string.</param>
        /// <param name="tag">The message's tag.</param>
        /// <param name="parentType">The type which contains static values to use for hole values.</param>
        public void WriteStructuredStaticParent(string template, string tag, Type parentType)
        {
            if (!Enabled)
            {
                return;
            }
            if (Formatter != null)
            {
                template = Formatter.Transform(template, this, tag, new Dictionary<string, string>());
            }
            
            var message = TemplateProcessor.BuildParentMessage(template, parentType);
            if(LogPool.StageMessage(this, message, tag))
            {
                return;
            }
            WriteStructuredMessage(message, tag);
        }

        /// <summary>
        /// Outputs a <see cref="StructuredMessage"/> to the log.
        /// </summary>
        /// <param name="message">The message to output.</param>
        /// <param name="tag">The message's tag.</param>
        public void WriteStructuredMessage(StructuredMessage message, string tag)
        {
            if (!Enabled)
            {
                return;
            }
            if (!Filter.ValidateTemplated(message, tag))
            {
                return;
            }
            lock (_sinks)
            {
                foreach (var sinkKeyValue in _sinks)
                {
                    if (sinkKeyValue.Value.Enabled && sinkKeyValue.Value.Filter.ValidateTemplated(message, tag))
                    {
                        sinkKeyValue.Value.ProcessMessage(message, tag);
                    }
                }
            }
        }


        //********************************************
        // WriteException
        //********************************************

        /// <summary>
        /// Prints an exception.
        /// </summary>
        /// <param name="exception">The exception to print.</param>
        public void WriteException(Exception exception)
        {
            WriteException(exception, false, "", false);
        }
        
        /// <summary>
        /// Prints an exception, optionally throwing the same exception.
        /// </summary>
        /// <param name="exception">The exception to print</param>
        /// <param name="throwException">A boolean value indicating if this method should throw the exception.</param>
        public void WriteException(Exception exception, bool throwException)
        {
            WriteException(exception, false, "", false);
        }
        
        /// <summary>
        /// Prints an exception with the specified tag, optionally throwing the same exception.
        /// </summary>
        /// <param name="exception">The exception to print</param>
        /// <param name="throwException">A boolean value indicating if this method should throw the exception.</param>
        /// <param name="tag">The tag to print with.</param>
        public void WriteException(Exception exception, bool throwException, string tag)
        {
            WriteException(exception, throwException, tag, false);
        }
        
        /// <summary>
        /// Prints an exception with the specified tag, optionally throwing the same exception and also optionally printing the call stack.
        /// </summary>
        /// <param name="exception">The exception to print</param>
        /// <param name="throwException">A boolean value indicating if this method should throw the exception.</param>
        /// <param name="tag">The tag to print with.</param>
        public void WriteException(Exception exception, bool throwException, string tag, bool printStack)
        {
            var builder = new StringBuilder();
            GetExceptionPrintout(builder, exception);

            if (printStack)
            {
                builder.AppendLine(exception.StackTrace);
            }

            WriteTag(builder.ToString(), tag);
            if (throwException)
            {
                throw exception;
            }
        }


        //********************************************
        // Internals
        //********************************************

        internal void PushMessage(string message, string tag)
        {
            lock (_listeners)
            {
                foreach (var listenerKeyValue in _listeners)
                {
                    if (listenerKeyValue.Value.Enabled && listenerKeyValue.Value.Filter.Validate(message, tag))
                    {
                        if(Formatter != null && listenerKeyValue.Value.FormatterArgs.Count > 0)
                        {
                            listenerKeyValue.Value.Write(Formatter.Transform(this, message, tag, listenerKeyValue.Value.FormatterArgs), tag);
                            continue;
                        }
                        listenerKeyValue.Value.Write(message, tag);
                    }
                }
            }
        }

        private void GetExceptionPrintout(StringBuilder builder, Exception exception, string indention = "")
        {
            if (exception == null)
            {
                return;
            }
            builder.AppendFormat("{0}{1}: {2}{3}", indention, exception.GetType(), exception.Message, Environment.NewLine);
            GetExceptionPrintout(builder, exception.InnerException, indention + "  ");
        }
    }
}
