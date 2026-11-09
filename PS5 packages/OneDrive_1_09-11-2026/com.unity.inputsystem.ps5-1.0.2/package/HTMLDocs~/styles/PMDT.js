// Unity-specific package manager doctool scripts

////////////////////////////////////////////////////////////////////////////
// Finds images above a threshold and adds zoom functionality to them.
// In this implementation, zoom functionality is a magnifier cursor and a link that opens the image
// in a new tab.
function addZoomToLargeImages()
{
    const threshold = $("meta[property='unity:image-zoom-threshold']").attr("content");

   //If the image threshold is greater than 0, then apply zoom treatment to images larger than threshold
    if(threshold > 0){ 
        const images = document.querySelectorAll("article :not(a):not(canvas) > img");
        for (let i = 0; i < images.length; i++) {
            let image = images[i];
            if(image.naturalWidth > threshold){
                image.style.cursor = "zoom-in";
                const a_link = document.createElement("a");
                a_link.href = image.getAttribute("src");
                a_link.target = "zoomed-images";
                image.parentNode.insertBefore(a_link, image);
                a_link.appendChild(image);
            }
        }
    }
}


////////////////////////////////////////////////////////////////////////////
// Open links to a different host in a new window/tab.
 function renderLinks() {
  const unityManualRegex = /docs\.unity3d\.com\/(?:[\d.]+\/Documentation\/)*Manual/;
  const unityScriptRefRegex = /docs\.unity3d\.com\/(?:[\d.]+\/Documentation\/)*ScriptReference/;
  const packageRegex = /docs\.unity3d\.com\/Packages\/([^@/]+)/;

  //Limiting the number of links as a failsafe to keep pages from becoming unresponsive
  if ($("meta[property='docfx:newtab']").attr("content") === "true" && $(document.links).length < 5000) {
    let currentPackage = "";
    if(packageRegex.test(window.location.href)) {
      currentPackage = window.location.href.match(packageRegex)[1];
    }
    for(let i = 0; i < $(document.links).length; i++){
      let link = $(document.links)[i];
      let href = link.href;
      if(link.hostname !== window.location.hostname) // to other host
      {
        link.target = link.hostname;
      } 
      else if(unityScriptRefRegex.test(href)) { // to core Script ref
        link.target = 'unity_api';
      }
      else if(unityManualRegex.test(href)) { // to core Manual
        link.target = 'unity_manual';
      }
      else if(packageRegex.test(href)) { // to other package
        let match = link.href.match(packageRegex);
        if(match[1] != currentPackage){
          link.target = match[1];
        }
      }
    }
  }
}

////////////////////////////////////////////////////////////////////////////
// Looks for Canvas elements of class image-comparison and sets them up to
// compare the two images with a slider.

// Contains the data for managing image comparison canvases
class ImageComparer {
    constructor(canvas, img1, img2, lbl1 = "", lbl2 = "", percent = .5) {
        this.canvas = canvas;
        this.img1 = img1;
        this.img2 = img2;
        this.lbl1 = lbl1;
        this.lbl2 = lbl2;
        this.percent = percent;
        this.thumbHeight = .5;
    }
}

// Finds canvases with the image-comparison class and sets them up
function setupImageComparers() {
    let canvases = document.querySelectorAll('.image-comparison');

    canvases.forEach(function (canvas, currentIndex, listObj) {
        let images = canvas.querySelectorAll('img');
        let imgComp = null;

        if(images.length <= 1)
            return;

        if(images.length >= 2){
            imgComp = new ImageComparer(canvas, images[0], images[1], images[0].title, images[1].title);
            imgComp.canvas.style.cursor = 'col-resize';
            imgComp.canvas.tabIndex = 0;
        }

        imgComp.canvas.width = Math.min(canvas.parentElement.clientWidth, imgComp.img1.naturalWidth);
        imgComp.canvas.height = imgComp.img1.naturalHeight * imgComp.canvas.width/imgComp.img1.naturalWidth;
        imgComp.canvas.style.touchAction = "none";
        drawImageCompare(imgComp);

        function onMove(e) {
            if(e.pointerType == "mouse" && e.buttons != 1)
                return;

            e.preventDefault();
            imgComp.percent = e.offsetX / canvas.width;
            imgComp.thumbHeight = e.offsetY / canvas.height;
            drawImageCompare(imgComp);
        }

        function onDown(e) {
            if(e.pointerType == "mouse" && e.buttons != 1)
                return;

            e.preventDefault();
            imgComp.canvas.focus();
            imgComp.canvas.setPointerCapture(e.pointerId);
            imgComp.percent = e.offsetX / canvas.width;
            imgComp.thumbHeight = e.offsetY / canvas.height;
            drawImageCompare(imgComp);
        }

        function onUp(e){
            imgComp.canvas.releasePointerCapture(e.pointerId);
        }

        function onResize(e) {
            imgComp.canvas.width = Math.min(canvas.parentElement.clientWidth, imgComp.img1.naturalWidth);
            imgComp.canvas.height = imgComp.canvas.width * imgComp.img1.naturalHeight/imgComp.img1.naturalWidth;
            drawImageCompare(imgComp);
        }

        function onFocusIn(e){
            imgComp.canvas.addEventListener('keydown', onKeydown, false);
        }

        function onFocusOut(e){
            imgComp.canvas.removeEventListener('keydown', onKeydown, false);
        }

        function onKeydown(e){
            if(e.key == "ArrowLeft" || e.key == "ArrowRight"){
                e.preventDefault();
                let step = e.shiftKey ? .1 : .01;
                imgComp.percent += e.key == "ArrowLeft" ? -step : step;
                imgComp.percent = Math.max(0, imgComp.percent);
                imgComp.percent = Math.min(1, imgComp.percent);
                drawImageCompare(imgComp);
            }
        }

        imgComp.canvas.addEventListener('pointermove', onMove, false);
        imgComp.canvas.addEventListener('pointerdown', onDown, false);
        imgComp.canvas.addEventListener('pointerup',   onUp,   false);    
        imgComp.canvas.addEventListener('focusin', onFocusIn, false);
        imgComp.canvas.addEventListener('focusout', onFocusOut, false);
        window.addEventListener('resize', onResize);
    });
};

// Draws the two images to be compared to the canvas with a draggable line.
function drawImageCompare(imgComp) {
    if(imgComp == null)
        return;

    let d = Math.max(0, imgComp.percent);
        d = Math.min(1, d);
    let ctx = imgComp.canvas.getContext('2d');
    let w = imgComp.canvas.width;
    let h = imgComp.canvas.height;
    let nw = imgComp.img1.naturalWidth;
    let nh = imgComp.img1.naturalHeight;

    ctx.clearRect(0, 0, imgComp.canvas.width, imgComp.canvas.height); 
    ctx.globalCompositeOperation = 'source-over';

    // Draw left image and label
    ctx.save();
        ctx.beginPath();
        ctx.rect( 0, 0, w * d, h);
        ctx.clip();
        ctx.drawImage(imgComp.img1, 0, 0, w, h);
        if(imgComp.lbl1)
            drawCanvasLabel(ctx, imgComp.lbl1, 10, h - 10, "left");
     ctx.restore(); // Clear the clipping area
    

    if(imgComp.img2 != null){ //Only draw the rest if there is a second image
        ctx.save(); //Clip the top image based on percentage d
            ctx.beginPath();
            ctx.rect( d * w, 0, w - w * d, h);
            ctx.clip();

            ctx.drawImage(imgComp.img2,  0, 0, w, h);
            if(imgComp.lbl2)
                drawCanvasLabel(ctx, imgComp.lbl2, w - 10, h - 10, "right");
        ctx.restore(); // Clear the clipping area to draw drag line

        // draw the draggable line and triangles
        ctx.save();
            ctx.shadowColor = 'black';
            ctx.shadowBlur = 4;

            ctx.fillStyle = 'white';
            ctx.fillRect(w * d - 1, 0, 2, h);

            // Position slider arrows at mouse yOffset, but not off canvas
            let th = imgComp.thumbHeight * h;
            th = Math.max(th, 15);
            th = Math.min(th, h - 15);
            // Draw left triangle
            ctx.beginPath();
            ctx.moveTo(w * d - 5, th - 10);
            ctx.lineTo(w * d - 15, th);
            ctx.lineTo(w * d - 5, th + 10);
            ctx.closePath();
            ctx.fill();

            // Draw right triangle
            ctx.beginPath();
            ctx.moveTo(w * d + 5, th - 10);
            ctx.lineTo(w * d + 15, th);
            ctx.lineTo(w * d + 5, th + 10);
            ctx.closePath();
            ctx.fill();
        ctx.restore();
    }
}

//Draw the labels from the image title attributes
function drawCanvasLabel(context, text, x, y, alignment = "left")
{
    context.font = "20px 'Roboto', sans-serif";
    let textMetrics = context.measureText(text);
    context.fillStyle = 'rgba(10, 10, 10, .6)';
    context.beginPath();
    if(alignment == "right")
        context.roundRect(x - textMetrics.width - 4, y - 20, textMetrics.width + 8, 28, [5]);
    else
        context.roundRect(x - 4, y - 20, textMetrics.width + 8, 28, [5]);
    context.closePath();
    context.fill();

    context.fillStyle = "white";
    context.textAlign = alignment;
    context.fillText(text, x, y);
}

////////////////////////////////////////////////////////////////////////////
// Add "Install this version" links to Changelog entries
function addInstallLinks(){
    if(!window.location.pathname.includes("/CHANGELOG.html"))
        return;
    
    // Pattern = digits.digits.digits followed by an optional - plus a sequence of any characters 
    // except `]`, all enclosed in `[]` 
    let versionPattern = /\[(\d+\.\d+\.\d+-*[^\]]*)\]/; 
    const packageID = $("meta[property='unity:packageID']").attr("content");
    const versionTitles = document.querySelectorAll("h2");

    for(let i = 0; i < versionTitles.length; i++){
        let versionText = versionTitles[i].textContent;
        let versionMatches = versionText.match(versionPattern);

        // There should never be more than one match, but this is JavaScript, so the "result" 
        // array has 2 members in this case.
        if(versionMatches != null && versionMatches.length == 2) 
        {
            let version = versionMatches[1];
            let link = document.createElement("a");
            link.innerText = "Add version " + version + " by name.";
            link.href = "com.unity3d.kharma:upmpackage/" + packageID + "@" + version;
            link.setAttribute("title", "Only compatible versions can be installed.");
            versionTitles[i].insertAdjacentElement("afterend", link);
        }
    }
}

////////////////////////////////////////////////////////////////////////////
// Process web page to add functionality
function enhancePage(){
  addZoomToLargeImages();
  renderLinks();
  setupImageComparers();
  addInstallLinks();
}

////////////////////////////////////////////////////////////////////////////
// Calls the page enhancement functions when the page is loaded
window.addEventListener('load', enhancePage);
