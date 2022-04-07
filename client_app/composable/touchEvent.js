// THIS IS A GENERIC PRESS AND HOLD FUNCTION
export default function touchFuncs(item) {
    let timerID;
    let counter = 0;
    let pressHoldEvent = new CustomEvent("pressHold");
    let pressHoldDuration = 50;
    console.log(pressHoldDuration)
    function pressingDown(e) {
        requestAnimationFrame(timer)
        e.preventDefault()
        console.log("Pressing!")
    }
    function notPressingDown(e) {
        // stop the timer
        cancelAnimationFrame(timerID)
        counter = 0
        console.log("Not pressing!")
    }
    function timer() {
        console.log("Timer tick!")

        if (counter < pressHoldDuration) {
            timerID = requestAnimationFrame(timer)
            counter++
        } else {
            console.log("long press duration reached")
            item.dispatchEvent(pressHoldEvent)
        }
    }
    return {
        pressingDown,
        timer,
        notPressingDown
    }
}