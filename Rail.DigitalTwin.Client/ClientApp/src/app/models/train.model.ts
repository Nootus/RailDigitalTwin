export interface TrainModel {
  trainID: number,
  trainNumber: number,
  trainName: string,
  speed: number, // kmph
  recommendedSpeed: number, //kmph
  trainLength: number // meters
  frontTravelled: number,  // meters
  rearTravelled: number,  // meters

  // angular only fields
  trainWidth: number, // pixels used for moving train
  trainLocation: number // pixels used for moving train
}
