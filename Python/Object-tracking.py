from pip._vendor.distlib.compat import raw_input
import cv2
import numpy as np
import time
import socket

# hsv default values for the red tracked ball
hsvMin = np.array([0, 165, 50])
hsvMax = np.array([180, 255, 255])
coord_list = []

# IMPORTANT -> udp_ip has been initialsed to 000.000.0.00, please change this to run this script

def main():
    xCoord, yCoord = 0, 0

    # User has to navigate through a quick configuration process, pressing enter each time if default
    # User is told to wait until the VR application tells them to return here, if they went through with
    # the configuration before the application was running there would be an OS error
    print("Before you proceed, open up the VR application, only then should you return here and press the Enter key")
    confirm_status(raw_input())

    print("Press enter to continue using the default tracking surface, else press q key")
    newXmax, newYmax = configure_surface(raw_input())

    print("Press enter to continue with default network settings, else press q")
    udp_ip, udp_port = configure_network(raw_input())

    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    # Camera is initialised and displays on the screen for the user to select the corners of
    # the surface they are tracking the ball on
    camera = cam_init()
    print("Click the 4 corners of your surface in the 'Select Boundary' window\n")
    while 1:
        re, frame = camera.read()
        cv2.imshow('Select Boundary', frame)
        cv2.setMouseCallback('Select Boundary', click_event)

        # Check that four corners have been selected or user has pressed q to quit
        if cv2.waitKey(1) == ord("q") or len(coord_list) == 4:
            break

    release_and_destroy(camera)
    camera = cam_init()

    # prompted to return to the VR headset as the configuration is complete, everything else
    # works by itself from now
    print("\nConfiguration complete, return to wearing the VR headset")

    # this while loop is where the object tracking takes place
    while 1:
        re, frame = camera.read()
        transformed_frame = transform_coords(frame)
        mask = create_mask(transformed_frame)

        # coordinates of the object are returned from circle object helper function
        xCoord, yCoord = circle_object(mask, transformed_frame, xCoord, yCoord)
        cv2.imshow('Transformed frame', transformed_frame)

        # coordinates are recalculated given the max coordinates given in surface configuration
        xCoord = (newXmax / 300) * xCoord
        yCoord = (newYmax / 420) * yCoord

        # < 0.01 inaccuracy exists with small numbers
        if xCoord > newXmax or xCoord < 0.01:
            xCoord = 0.0
        # < 0.01 inaccuracy exists with small numbers
        if yCoord > newYmax or yCoord < 0.01:
            yCoord = 0.0

        # the coordinates are combined into a string object which is to be sent via UDP
        coordString = str(yCoord) + "," + str(xCoord)

        # the UDP packet containing the coordinate information is sent here
        sock.sendto(coordString.encode(), (udp_ip, udp_port))

        if cv2.waitKey(1) == ord("q"):
            break

    release_and_destroy(camera)


def confirm_status(sel):
    if sel == "":
        print("---------------------Configuration---------------------")


# this function handles the user changing the dimensions of the surface
# that the object is being tracked on top of
def configure_surface(sel):
    if sel == "":
        newYmax, newXmax = 1, 0.6
        print("Desk selected\n")
    if sel == "w":
        newYmax, newXmax = 0.86, 0.56
        print("Whiteboard Selected")
    if sel == "q":
        print("Type the length of y in metres")
        newXmax = raw_input()
        print("Type the length of x in metres")
        newYmax = raw_input()
        print("Press enter to confirm new surface dimensions: " + newYmax + " x " + newXmax + ", else press q")
        if raw_input() == "q":
            configure_surface(sel)
    return newXmax, newYmax


# this function configures the network IP and port settings for the UDP transfer
def configure_network(sel):
    if sel == "":
        udp_ip, udp_port = "000.000.0.00", 8051
    if sel == "q":
        print("Type new IP then press enter")
        udp_ip = raw_input()
        print("Type new port then press enter")
        udp_port = raw_input()
    print("UDP target IP: " + str(udp_ip) + ", UDP target port: " + str(udp_port) + "\n")
    return udp_ip, udp_port


# initialise the camera object
def cam_init():
    c = cv2.VideoCapture(0)
    time.sleep(2.0)
    return c


# event when the user clicks a corner of the surface -> corner is added to a list of coordinates
def click_event(event, x, y, flags, param):
    if event == cv2.EVENT_LBUTTONDOWN:
        coord_list.append(([x, y]))
        print("Coordinates {}/4: {} ".format(len(coord_list), coord_list))


# this function undoes the skew in the image of the surface that occurs due to the camera mounting position
def transform_coords(frame):
    orig_coords = np.float32([[coord_list[0][0], coord_list[0][1]], [coord_list[1][0], coord_list[1][1]],
                              [coord_list[2][0], coord_list[2][1]], [coord_list[3][0], coord_list[3][1]]])
    new_coords = np.float32([[0, 0], [300, 0], [0, 420], [300, 420]])
    matrix = cv2.getPerspectiveTransform(orig_coords, new_coords)
    transformed_frame = cv2.warpPerspective(frame, matrix, (300, 420))
    return transformed_frame


# this function circles the object based on contours that are detected within the masked image
def circle_object(mask, transformed_frame, xCoord, yCoord):
    contours, _ = cv2.findContours(mask, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
    contours = sorted(contours, key=lambda a: cv2.contourArea(a), reverse=True)
    for c in contours:
        ((x, y), radius) = cv2.minEnclosingCircle(c)
        cv2.circle(transformed_frame, (int(x), int(y)), int(radius), (255, 255, 0), 3)
        xCoord, yCoord = round(x, 2), round(y, 2)
        break
    return xCoord, yCoord


# this function HSV masks the image, and erosion and dilation takes place to smooth out the detected
# object and remove inconsistencies
def create_mask(frame):
    frame = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)
    mask = cv2.inRange(frame, hsvMin, hsvMax)
    mask = cv2.erode(mask, np.ones((15, 15)))
    mask = cv2.dilate(mask, np.ones((4, 4)))
    return mask


# the camera object is released and destroyed
def release_and_destroy(camera):
    camera.release()
    cv2.destroyAllWindows()

    # Bug fix to kill the windows
    for i in range(1, 5):
        cv2.waitKey(1)


if __name__ == "__main__": main()
