//
// Copyright 2020 Electronic Arts Inc.
//
// The Command & Conquer Map Editor and corresponding source code is free 
// software: you can redistribute it and/or modify it under the terms of 
// the GNU General Public License as published by the Free Software Foundation, 
// either version 3 of the License, or (at your option) any later version.

// The Command & Conquer Map Editor and corresponding source code is distributed 
// in the hope that it will be useful, but with permitted additional restrictions 
// under Section 7 of the GPL. See the GNU General Public License in LICENSE.TXT 
// distributed with this program. You should have received a copy of the 
// GNU General Public License along with permitted additional restrictions 
// with this program. If not, see https://github.com/electronicarts/CnC_Remastered_Collection
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace MobiusEditor.Utility {
    public class TrackablePropertyDescriptor<T> : PropertyDescriptor {
        private readonly T obj;
        private readonly PropertyInfo propertyInfo;
        private readonly Dictionary<string, object> propertyValues;

        public override Type ComponentType => this.obj.GetType();

        public override bool IsReadOnly => false;

        public override Type PropertyType => this.propertyInfo.PropertyType;

        public TrackablePropertyDescriptor(string name, T obj, PropertyInfo propertyInfo, Dictionary<string, object> propertyValues)
            : base(name, null) {
            this.obj = obj;
            this.propertyInfo = propertyInfo;
            this.propertyValues = propertyValues;
        }

        public override bool CanResetValue(object component) => this.propertyValues.ContainsKey(this.Name);

        public override object GetValue(object component) {
            if(this.propertyValues.TryGetValue(this.Name, out var result)) {
                return result;
            }
            return this.propertyInfo.GetValue(this.obj);
        }

        public override void ResetValue(object component) => this.propertyValues.Remove(this.Name);

        public override void SetValue(object component, object value) {
            if(Equals(this.propertyInfo.GetValue(this.obj), value)) {
                this.propertyValues.Remove(this.Name);
            } else {
                this.propertyValues[this.Name] = value;
            }
        }

        public override bool ShouldSerializeValue(object component) => false;
    }

    public class PropertyTracker<T> : DynamicObject, ICustomTypeDescriptor {
        private readonly Dictionary<string, PropertyInfo> trackableProperties;
        private readonly Dictionary<string, object> propertyValues = new Dictionary<string, object>();

        public T Object {
            get; private set;
        }

        public PropertyTracker(T obj) {
            this.Object = obj;

            this.trackableProperties = this.Object.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => (p.GetGetMethod() != null) && (p.GetSetMethod() != null))
                .ToDictionary(k => k.Name, v => v);
        }

        public void Revert() => this.propertyValues.Clear();

        public void Commit() {
            foreach(var propertyValue in this.propertyValues) {
                this.trackableProperties[propertyValue.Key].SetValue(this.Object, propertyValue.Value);
            }
            this.propertyValues.Clear();
        }

        public IDictionary<string, object> GetUndoValues() => this.propertyValues.ToDictionary(kv => kv.Key, kv => this.trackableProperties[kv.Key].GetValue(this.Object));

        public IDictionary<string, object> GetRedoValues() => new Dictionary<string, object>(this.propertyValues);

        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            if(!this.trackableProperties.TryGetValue(binder.Name, out var property)) {
                result = null;
                return false;
            }

            if(!this.propertyValues.TryGetValue(binder.Name, out result)) {
                result = property.GetValue(this.Object);
            }
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value) {
            if(!this.trackableProperties.TryGetValue(binder.Name, out var property)) {
                return false;
            }

            if(Equals(property.GetValue(this.Object), value)) {
                this.propertyValues.Remove(binder.Name);
            } else {
                this.propertyValues[binder.Name] = value;
            }
            return true;
        }

        public AttributeCollection GetAttributes() => TypeDescriptor.GetAttributes(this.Object.GetType());

        public string GetClassName() => TypeDescriptor.GetClassName(this.Object.GetType());

        public string GetComponentName() => TypeDescriptor.GetComponentName(this.Object.GetType());

        public TypeConverter GetConverter() => TypeDescriptor.GetConverter(this.Object.GetType());

        public EventDescriptor GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(this.Object.GetType());

        public PropertyDescriptor GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(this.Object.GetType());

        public object GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(this.Object.GetType(), editorBaseType);

        public EventDescriptorCollection GetEvents() => TypeDescriptor.GetEvents(this.Object.GetType());

        public EventDescriptorCollection GetEvents(Attribute[] attributes) => TypeDescriptor.GetEvents(this.Object.GetType(), attributes);

        public PropertyDescriptorCollection GetProperties() {
            var propertyDescriptors = this.trackableProperties.Select(kv => {
                return new TrackablePropertyDescriptor<T>(kv.Key, this.Object, kv.Value, this.propertyValues);
            }).ToArray();
            return new PropertyDescriptorCollection(propertyDescriptors);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) => this.GetProperties();

        public object GetPropertyOwner(PropertyDescriptor pd) => this.Object;
    }
}
