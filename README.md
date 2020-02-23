# SpeakEasy

Practice your speech in front of an audience and receive real time feedback on your vocal and physical delivery

[Youtube Video Link](https://www.youtube.com/watch?v=g6q7rjH1Efc&t=3s)

[Devpost Link](https://devpost.com/software/speakeasy-5wl7z2)

## Grand Prize Winner of Reality Virtually Hackathon 2017

![winner](https://pbs.twimg.com/media/DLvscVLUMAI2B0H?format=jpg&name=4096x4096)

## Inspiration
With VR and AR, we can both mimic the stressful situations that trigger your physical and vocal nervous ticks, and give you real time feedback to help you eliminate them. Additionally, we can put you in the shoes of the great public speakers in history, and teach you the vocal and physical delivery that made their speeches great.

## What it does
With Speakeasy you you can choose the type of public speaking you want to practice, to help you overcome your fears and get hints on how to become a master public speaker. The first step is choosing whether you are delivering a public speech, a classroom lecture, answering interview questions, or asking someone out on a date. After you choose your scene, you are transported to that environment in VR utilizing 360 video that we captured using a Ricoh Theta camera during the Hackathon.

Once you are transported to the scene of your choosing, you begin delivering your speech. Unlike the commercial solutions which currently exist, you get feedback in real time on your vocal AND physical delivery.

On the vocal side, Speakeasy immediately alerts you if you use a filler words like "umm", "right" or "you know" and keeps track of how many times you used each filler word. Speakeasy also tracks how many words you are speaking per minute, and lets you know if you are going to fast or too slow. And finally, Speakeasy tracks the tone and loudness of your voice in real time, so you can find out if you are varying your frequency correctly and projecting optimally.

On the physical side, Speakeasy tracks every gesture you make via a Leap Motion detector, so you can both see your gestures in VR and ensure you are doing enough gestures per minute. The best public speakers tell a story both through their words and their physical movement. If you don't do enough gestures, Speakeasy lets you know. Additionally, Speakeasy can track if you lean from side to side, and don't keep eye contact with the audience.

The feedback provided by Speakeasy is based on scientific research into communication best practices. Specifically, a survey of all the TED talks posted online found that the speakers of the most viral TED talks, which on average received 7.4 million views, used on average 26 gestures per minute. On the other hand, the least viral TED talks, which on average received only 120,000 views, the speakers did only 15 gestures per minute. When we look at the vocal patterns of the masters, on average they use 130 words per minute, and vary their vocal tone.

After you finish your speech, you receive a summary screen, letting you know how your speech will be received, and tips to improve your delivery. You can also share you speech with friends or a professional coach, so you can receive feedback on your content and overall performance. These ratings will later be fed back to Speakeasy's machine learning algorithm, so it can learn the unique delivery style that works best for you.

From the main menu you can also go back into your speech "History", in case you said a phrase or did a gesture you want to remember, and go into "Master" mode to see how well you can deliver the best speeches in history.

## How we built it
We tackled providing real time feedback of someones vocal delivery first. Eliminating the filler words was our first and primary goal, and accomplishing just that in an immersive VR or AR environment we felt would make our hack a success. We did all our audio processing inside a locally running python server that utilizes pyaudio and the Google Speech to Text API. With the Speech to Text API we can get a full transcript of the words used throughout someones speech, and with a blacklist of the most commonly used filler words, detect when someone said a filler word. In our python server we also track the loudness of the users voice. We calculate loudness using an RMS algorithm which we wrote, where we only track the loudness of each word spoken and ignore the gaps in speech when the user is pausing between words. This was vitally important so that we determine the true vocal variation of the user and not penalize them for taking pauses for dramatic effects, which the speaker is encouraged to do by Speakeasy. Additionally, when calculating Loudness we average the loudness over time, so score how well the user is projecting himself and whether they are varying their tone well. Averaging over time was important so that Speakeasy doesn't give false warnings or praise based on too short of a time frame. Finally our python server sends the information to our Unity app via HTTP requests, updating Unity each time a word is spoken.

The core of the VR/AR experience was built in Unity utilizing an HTC Vive and Leap Motion hand trackers. With the Leap Motion detector, we were able to determine each time a user did a gesture in the "strike zone". Multiple studies have shown that the ideal number of gestures is 26 per minute, and that the gestures should be made in front of the user in an area similar to a baseball "strike zone" (http://wapo.st/2z8RZjh). Only gestures made in this "strike zone" are counted as "Good" gestuers by Speakeasy, so if a user is making gestures while the hands are very low (so too weak of a gesture) or the hands are too high (too aggressive of a gesture), those "Bad" gestures are ignored and don't fall into the "Good" category. Additionally, with the HTC Vive headset we are able to track whether a user is making a rocking motion as well as whether they are utilizing the stage effectively if the speaker intends to walk around the stage. Finally, with the HTC Vive hand controller, we mimic a 3D microphone, so a user can practice the delivery of a speech when they know they will be using a microphone.

To create the scenes that mimic the different public speaking environments that a user may encounter, we utilized a Ricoh Theta camera, and filmed scenes inside the MIT Stata Center Keynote auditorium from the hackathon. Additionally, we asked the audience of hackers inside the Keynote auditorium to give us different reactions, including clapping, booing, laughing, slowly ignoring the speaker and paying attention. With these different 360 video clips, we strung them together inside Unity to give the HTC Vive user a first person perspective of delivering a keynote speech at the Reality Virtually Hackathon. Beyond this, by capturing different audience reactions, we can give the speech giver a realistic audience reaction when they start using filler words and take on other bad habits. In those instances the audience will boo. When they begin hitting all the optimal delivery techniques for physical and vocal delivery, the audience will be focused on them. We ultimately brought the 360 video into Unity with a Sphere object, texture, material, and a skybox.

To construct the UX of our scenes, the initial mockups were made with Photoshop and then implemented in Unity.

We also did initial testing of the AR experience with Meta, Samsung Gear and Merge VR headsets, and look forward to bringing the Speakeasy app to those platforms in the future, so that users can receive the real time feedback which Speakeasy provides while practicing their speeches in the real environment and in front of a real, live audience.

## Challenges we ran into
Our team had never used Leap Motion before, stitched together a 360 video inside Unity and had limited experience with the signal processing and machine learning necessary for the vocal feedback.

The Leap Motion tracking while initially easy to get set up, was challenging to determine the best way to track what we would consider an optimal "strike zone" gesture.

Importing 360 video into Unity, and using it in the context of a HTC Vive scene ended up taking much, much longer than we expected. There was a script that we were missing that we didn't realize we needed, and it took many iterations and multiple tutorials to determine that.

## Accomplishments that we're proud of
We came together quickly as a team, and were able to immediately focus on a clear goal for a hack and be productive within 5 minutes of forming a team. This was especially impressive as none of us came into the hackathon knowing what we wanted to work on, and we also met for the first time at the hackathon.

After forming our team, because we were so focused and able to communicate what we wanted to do effectively to each other, we were able to capture 360 video from the tail end of the team building in the Reality Virtually Keynote auditorium so we could get audience reactions to be used in our VR experience.

Additionally, we are able to accomplish our primary and stretch goals in terms of audio and physical gesture processing!

## What we learned
How to use:

Leap Motion
Python sound processing libraries
Creating a 360 video skybox
Gesture tracking
And a lot more!

## What's next for Speakeasy

Now that we can do the Vocal and Gesture tracking, we want to put you in the shoes of master public speakers, and gamify the practice of trying to speak like great speakers in history. Step into the shoes of Steve Job's when he launched the iPhone to the world, and see how well you can mimic his keynote speech by saying the words he said and making the gestures he made.

Additionally, we want to bring Speakeasy to Meta, Samsung Gear VR and Merge VR so you can get real time feedback while practicing your speech out in the Real World.
