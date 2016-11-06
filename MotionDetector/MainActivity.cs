using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Hardware;

namespace MotionDetector
{
    [Activity(Label = "MotionDetector", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ISensorEventListener
    {
        static readonly object _syncLock = new object();
        SensorManager _sensorManager;
        TextView _sensorTextView;
        TextView _sensorFilter;
        Button _btnSend;

        float[] _prevValues;
        float[] _currValues;
        const float alfa = 0.1f;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _prevValues = new float[3];
            _currValues = new float[3];
            for (int i = 0; i < 3; i++)
            {
                _prevValues[i] = 0.0f;
                _currValues[i] = 0.0f;
            }

            SetContentView(Resource.Layout.Main);
            _sensorManager = (SensorManager)GetSystemService(SensorService);
            _sensorTextView = FindViewById<TextView>(Resource.Id.accelerometer_text);
            _sensorFilter = FindViewById<TextView>(Resource.Id.accelerometer_filter);
            _btnSend = FindViewById<Button>(Resource.Id.btnSend);
            _btnSend.Click += _btnSend_Click;
        }

        private void _btnSend_Click(object sender, EventArgs e)
        {
            var client = new SyncClient();
            //client.OnResponse += Client_OnResponse;
            client.StartClient(_sensorFilter.Text);
        }

        private void Client_OnResponse(object sender, System.Text.StringBuilder e)
        {
            _btnSend.Text = e.ToString();
        }

        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
            // We don't want to do anything here.
        }

        public void OnSensorChanged(SensorEvent e)
        {
            lock (_syncLock)
            {
                var raw = e.Values;
                // apply filter
                for (var i = 0; i < 3; ++i)
                    _currValues[i] = (1.0f - alfa) * _prevValues[i] + alfa * raw[i];

                _sensorTextView.Text = string.Format("x={0:f}, y={1:f}, y={2:f}", raw[0], raw[1], raw[2]);
                _sensorFilter.Text = string.Format("x={0:f}, y={1:f}, y={2:f}", _currValues[0], _currValues[1], _currValues[2]);

                // store previous values
                for (var i = 0; i < 3; ++i)
                    _prevValues[i] = _currValues[i];
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            _sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Accelerometer), SensorDelay.Ui);
        }

        protected override void OnPause()
        {
            base.OnPause();
            _sensorManager.UnregisterListener(this);
        }
    }
}

