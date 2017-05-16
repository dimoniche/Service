// Canvas
var canvas = document.getElementById('canvas');
var ctx = canvas.getContext('2d');
var cw = canvas.width;
var ch = canvas.height;
var strokeWidthFactor = 0.45;
var strokeWidth = strokeWidthFactor;
var circleArrSetting = [];
var timerCount = 0;

// Const
var color = '#a5dcfb';
var time = canvas.getAttribute('data-time');
var circleCount = canvas.getAttribute('data-count');

// Interval
var interval = 2/time;
var start = interval;

function init () {
	ctx.lineWidth = cw * .03;
	ctx.strokeStyle = color;

	for (var i = 0; i < circleCount; i++) {
		circleArrSetting.push({
			active: false,
			end: false
		});
		ctx.beginPath();
		ctx.arc(cw/2, ch/2, cw * strokeWidth, 0, 2*Math.PI, true);
		ctx.stroke();
		strokeWidth -= .05;

		setTimeout(function() {
			if(timerCount > 0) {
				circleArrSetting[timerCount-1].active = false;
				circleArrSetting[timerCount-1].end = true;
			}
			circleArrSetting[timerCount].active = true;
			// Countdown
			var countdown = setInterval(function () {
				draw(start*Math.PI);
				start += interval;
			}, 1000);
			
			// Stop countdown
			setTimeout(function() {
				clearInterval(countdown);
			}, time*1000);
			timerCount += 1;
		}, time*1000*i);
	}
}

function draw(end) {
	var currentEnd = end;
	strokeWidth = strokeWidthFactor;
	ctx.clearRect(0,0,canvas.width,canvas.height);
	for (var i = 0; i < circleCount; i++) {
		if(!circleArrSetting[i].active && !circleArrSetting[i].end)
			currentEnd = 2*Math.PI;
		if(circleArrSetting[i].active && !circleArrSetting[i].end)
			currentEnd = end;
		if(!circleArrSetting[i].active && circleArrSetting[i].end)
			currentEnd = 0*Math.PI;
		ctx.beginPath();
		ctx.arc(cw/2, ch/2, cw * strokeWidth, 0, currentEnd, true);
		ctx.stroke();
		strokeWidth -= .05;
	}
}

init ();