import json


class Message:
    @classmethod
    def from_json(cls, json_string):
        message_dict = json.loads(json_string)
        if not isinstance(message_dict, dict):
            raise ValueError(f"malformed JSON, cant convert {json_string} to {cls}")
        return cls.from_dict(message_dict)

    @classmethod   
    def from_dict(cls, message_dict):
        return cls(**message_dict)

    def to_json(self):
        return json.dumps(self.__dict__)
