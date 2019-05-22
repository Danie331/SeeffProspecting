// new icon + SS icon, filtering, update history tab, web hooks (status of listing?, reactivate listing, audit log
// Highlight the huge importance of keeping our listing statuses in sync with theirs.
// Double check whether Reload Owers + updating ownership + re-prospecting a sectional title can purge the listing_id (if present)
// Address workflow in terms of lifecycle: when property's ACTIVE status changes to inactive, null out the record in the property table and update record in listing table.
// make sure that the listing_id is updated on the front-end for the currentProperty when creating a listing.
// null out the listing_id on property record when deactivating + update log tbl
// validate listing fields + summary of listing to be created
// initial email to janathan
// create an activity for new listings

// validate max lengths + required fields (show red asterisk) + encode special characters.

//1	Get token from agents service / public - api / login "token_id" of "agents" property(Seeff Property Group)
//2	Use the token in the 1st call to service above for the next call, in ALL subsequent calls use token returned as part of that call
//3	Use the api / v1 / renew - token as a substitude for the above(20 minute timeout)								
//4	associated_contacts can be empty object(not required)
//5	location: seeff area table 1..n relationship with propdata areas
//5.1	locations / api / v1 /...seeff area mapping to property24 area mapping; use locations service query in slack channel passing the matched property 24 suburb - to - seeff suburb, get back a list and present user with options to make the final choice, use id of selected suburb

//agents / branches
//create a mapping from seeff license table to propdata's branch table								
//--poll propdata get_all locations service once daily to receive a data dump of all locations
//--the data dump will need to be retrofitted to update seeff's existing mapping								
//use the id from the mapping correlation table - simple!
//for agents and branches, pull list from propdata service and allow user to select a value(use id)
//UPDATES:
//use the slack example to query all updates after a certain date range...


// 1. implement fields for each Category
// 2. Next >> validate required fields  + lengths + readonly fields on summary: lightstone_id, erf, lat/lng
// 3. Show summary dialog.

// create new activity types[Property Listed], publish it on the audit log + ensure can filter by it

// Switch over to production URLs when ready.

// exception logging mechanism for api errors >> serialize the model json and log it in case of exception