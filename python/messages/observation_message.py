import json

class ObservationMessage:
    def __init__(self, agentPostion, agentRotation, velocity, angularVelocity, 
                 redBallPosition, blueBallPosition, greenBallPosition, 
                 grayAreaPosition, orangeAreaPosition, whiteAreaPosition,
                 agentRedBallAngle, agentGrayAreaAngle, agentRedBallDist,
                 agentGrayAreaDist, RedBallGreyAreaDist, agentOutsidePlane,
                 redBallOutsidePlane,terminate, reward):
        
        self.agentPostion = agentPostion
        self.agentRotation = agentRotation
        
        self.velocity = velocity
        self.angularVelocity = angularVelocity
        
        self.redBallPosition = redBallPosition
        self.blueBallPosition = blueBallPosition
        self.greenBallPosition = greenBallPosition
        
        self.grayAreaPosition = grayAreaPosition
        self.orangeAreaPosition = orangeAreaPosition
        self.whiteAreaPosition = whiteAreaPosition

        self.agentRedBallAngle = agentRedBallAngle
        self.agentGrayAreaAngle = agentGrayAreaAngle
        self.agentRedBallDist = agentRedBallDist
        self.agentGrayAreaDist = agentGrayAreaDist
        self.RedBallGreyAreaDist = RedBallGreyAreaDist
        self.agentOutsidePlane = agentOutsidePlane
        self.redBallOutsidePlane = redBallOutsidePlane
        
        self.terminate = terminate
        self.reward = reward
        
    @staticmethod
    def from_json(json_string):
        message_dict = json.loads(json_string)
        return ObservationMessage(**message_dict)
        """                          message_dict["agentPostion"], message_dict["agentRotation"],
                                  message_dict["velocity"], message_dict["angularVelocity"],
                                  message_dict["redBallPosition"], message_dict["blueBallPosition"],
                                  message_dict["greenBallPosition"], message_dict["grayAreaPosition"],
                                  message_dict["orangeAreaPosition"], message_dict["whiteAreaPosition"],
                                  message_dict["terminate"], message_dict["reward"])"""

    def to_json(self):
        return json.dumps(self.__dict__)
