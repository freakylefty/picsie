# Picsie #

Picsie is an ultra-lightweight media browser.

* See [Pixieland](http://www.pixieland.org.uk/picsie/) for more details.
* Contact [Andy](mailto:andy@aburn.org.uk)

* * *

Picsie is a very lightweight image and video viewer, with an emphasis on speed and minimal interface.   I primarily made it because while in the past I was perfectly happy with XnView, Quick Look on the Mac made it obvious how much Windows was missing something similar.

A while later I saw VJPEG which did pretty much exactly what I wanted.  Unfortunately it lacks a few features I like, so I set about making my own.

The result is Picsie.  It opens .bmp, .gif (including animated GIFs), .jpg, .jpe, .jpeg, .png, .if, .tiff, .avi, .mpg and .wmv files.  There is essentially no UI to it – simply a picture (or movie) on your desktop, with a drop shadow if your version of Windows supports it.  If it’s larger than your screen, it will be scaled to fit.  If you move away from the window, it will fade slightly.

##Controls

###Basics
- Drag: Move the window
- Right click: Exit
- Mouse wheel: Zoom
- Double-click: Centre and zoom 1:1
- Escape: Exit
- ‘t’: Toggle whether or not Picsie stays on top of other windows
- ‘b’ / ‘f’: Toggle fading if application loses focus
- ‘h’ / F1 / ‘?’: Show this help page

###File browsing
- Left arrow: Load previous file in folder
- Right arrow: Load next file in folder
- Up arrow: Load first file in folder
- Down Arrow: Load last file in folder
- Ctrl+Left arrow: Load first file in previous “group”
- Ctrl+Right arrow: Load first file in next “group”

Groups of files are defined as a name followed by a number, such as “screenshot 1.png” or “photo 9.jpg”.

###Animated GIFs
- ‘0’ (Zero): Pause animation
- ‘-‘: Reduce animation speed
- ‘+’ / ‘=’: Increase animation speed
- Backspace: Reset animation speed
- ‘[‘: Step frame backwards
- ‘]’: Step frame forwards

###Video Files
- Space: Pause/Play video
- ‘m’: Toggle sound

* * * 

If you use the installer from the website it will set up your file associations for you.  It plays nicely with other applications and just adds itself to the list of programs which can handle the file formats, rather than overriding everything.

If you load a large file (more than 10MB) Picsie prompts you rather than loading it immediately.  In this case you will get a message telling you to press return to view the file.

Lastly, if you open a corrupted or invalid file, or Picsie just has a problem (temperamental beast that it is), you’ll get an error message.  Feel free to complain at that point.