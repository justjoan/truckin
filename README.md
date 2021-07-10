# Truckin
## Overview
In addition to being a shout out to the iconic San Francisco band The Grateful Dead, *Truckin* is a mobile app set out to feed the busy people of San Franscico with ease. The app shows nearby food trucks through both an interactive map view and a list view, while offering lots of potential for future extension.  

## Considerations and Assumptions
- The only hard requirement is to provide the user with a minumum of 5 food trucks near their current location. 
- Assume fairness is important. Any cutoff applied should ideally be by distance vs an arbitrary number of results.
- The primary use case is targeting:
  - pedestrians (foot, bike, skateboard, wheelchair, scooter, etc) or a short commute (local transport, rideshare)
  - mobile devices
  - the user's current time and location vs a future time and location
  - convenience and proximity
- Possible user stories:  
  - A user is less familiar with the area and\or wants to visualize the locations on a map as part of their decision process, e.g. stay on this side of the highway, avoid that touresty area, I want somewhere on the way to my next destination\errand\park. 
  - A user knows the immediate area very well and can make decisions about location by address alone. They would prefer to see details at a glance and may never check a map.
  - A user strongly prefers type of food offerings over location and would like to see truck names and descriptions at a glance.  They will decide first and then check location on a map. 
  - A user is seeking food at an odd day\hour and assumes many places may not be open\available and would like to see hours at a glance and then check location on a map. 

## Approach 
### UX: Mobile app with primary map view and secondary list view
- This is not a complicated UI experience. Should be able to build a canvas mobile app with two views: a interactive map view and a list view, both tied to the same data source. TBD on navigation between the two views.
- Data load
  - Request: 
    - providing the starting latitute and longitude (user must allow sharing current location while app in use) and optionally whether to truncate results.
  - Response:
    - initial threshold (meters from starting point)
    - array of truck data (optionally only those within the threshold depending on performance factors)

- Map View (control view):
  - Center to user location and load pins and info cards for entries within radius
  - TBD: Do we get a lot of native behavior for free?  How is performance for showing\loading pins during zoom\pan?  Might it be better to fix a radius in increments?
  - Option to reset view to initial radius
- List View:
  - Bind list to pins displayed in map view (trigger update with zoom and pan)

### Service 
Endpoint = GET truckin/trucks?startLat=val,startLong=val,truncate=false)

1. Loads source data and fields from endpoint:
- Application (truck name) 
- FoodItems
- Address
- dayshours
- FacilityType
- Location (lat,long)
2. Filter out non-active entries (where Status is not APPROVED)
3. Sort list of trucks by distance from current location (nearest to farthest) 
4. Calculate initial radius of the map view to 180 meters (appx 2 city blocks) past the 5th nearest food truck.
5. Return:
- Initial radius threshold (distance containing at least 5 food trucks plus 2 block margin)
- Array of approved entries including the following data fields from the published CSV: 
  - Application (truck name)
  - FoodItems
  - dayshours
  - Address
  - FacilityType (truck, cart or default pin icon)
  - Distance 
- If truncating, limit to entries within the radius threshold, else return all for better pan\zoom support? 
  - TBD: not sure how native mapping handles data load and needs further evaluation on performance of map vs api roundtrip.  

## Project knowns and unknowns
- The project included here is an untested web service to fulfil the data request from the mobile app. My local environment is sorely lacking for being able to build out the solution in a more complete fashion. At this point there are still lots of questions and reasearch I would do, such as:
- Experiment with the mapping and geospacial features in the Power Platform to understand options for interacting with a data source, performance considerations, display formatting, info cards
- I doubt we'd want each phone calling out to the SFO website for data so caching of the original source data is a must but in current approach I'm not clear how connectors are hosted. I asssume instances are short lived so a memory cache won't work. What are the caching options in the platform?  Also need logic for expiring\refreshing the cache so would want to understand the rate of change or a method of change detection. Where data can be cached may affect decisions about how the data is sourced to the app, and visa versa.
- Would want to validate the assumptions made in the data around Status=APPROVED being the correct way to validate active trucks
- Is there any kind of data coming out of usage that would be helpful to the user? to the city? to the developent team?
- What's the right place to put the data endpoint url? Currently hardcoded but don't want to expose it for tampering either. Possibly depends on the caching solution - if the service remains it would read from the cache and another component would read from the SFO data source and update the cache. 
- Plus more, I'm sure...


## Future and Other Considerations
1. If native mapping features are not "free" or reloading pins and list during pan & zoom is clunky, consider options to increase initial radius in 1/2 mile increments.  If we do that should pan\zoom be disabled? 
2. Add logic around "dayshours" field, e.g. omit closed locations, filter by open locations, icon for closing soon. 
3. Consider filtering or setting pin icon to indicate category of food from CSV (data dependent).
4. Consider filtering by dietary restrictions or payment options (data dependent).
5. Are there mapping features that allow us to determine pedestrian inhibitors (highways, rivers, railyard).
6. Consider allowing user to tag favorites (authentication and data storage).
7. Smarter detection of data changes and refresh.
8. Connect calendar appointments that have a local address specified.
9. Consider more sophisticated algorithm to address density and sparcity while still being fair.
10. Language translations for "FoodItems" 
11. Voice commands for filtering by "FoodItems" content (e.g. find me a burger nearby)
12. Audio options for visually impaired



