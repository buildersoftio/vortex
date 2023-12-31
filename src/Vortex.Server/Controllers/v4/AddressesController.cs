﻿using Vortex.Core.Abstractions.Services;
using Vortex.Core.Models.Common.Addresses;
using Vortex.Core.Models.Dtos.Addresses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Vortex.Server.Controllers.v4
{
    [Route("api/v4/addresses")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly ILogger<AddressesController> _logger;
        private readonly IAddressService _addressService;

        public AddressesController(ILogger<AddressesController> logger, IAddressService addressService)
        {
            _logger = logger;
            _addressService = addressService;
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> PostAddress([FromBody] AddressCreationRequest addressCreationRequest)
        {
            if (addressCreationRequest == null)
                return BadRequest("Body request cannot be null");

            (bool isCreated, string message) = _addressService.CreateAddress(addressCreationRequest, "system");
            if (isCreated)
                return Ok(message);

            return BadRequest(message);
        }

        [HttpPost("create/default")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> PostDefaultAddress([FromBody] AddressDefaultCreationRequest addressCreationRequest)
        {
            if (addressCreationRequest == null)
                return BadRequest("Body request cannot be null");

            (bool isCreated, string message) = _addressService.CreateDefaultAddress(addressCreationRequest, "system");
            if (isCreated)
                return Ok(message);

            return BadRequest(message);
        }

        [HttpGet("byaddress/{addressName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin,Readonly")]
        public ActionResult<AddressDto> GetAddressByName(string addressName)
        {
            string addressNameUnescape = Uri.UnescapeDataString(addressName);

            (var address, string message) = _addressService.GetAddressByName(addressNameUnescape);
            if (address == null)
                return NotFound(message);

            return Ok(address);
        }

        [HttpGet("byalias/{alias}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin,Readonly")]
        public ActionResult<AddressDto> GetAddressByAlias(string alias)
        {
            (var address, string message) = _addressService.GetAddressByAlias(alias);
            if (address == null)
                return NotFound(message);

            return Ok(address);
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin,Readonly")]
        public ActionResult<AddressDto> GetAddresses()
        {
            (var addresses, string message) = _addressService.GetAddresses();
            if (addresses == null)
                return NotFound(message);

            return Ok(addresses);
        }

        [HttpPut("{alias}/settings/storage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> PutAddressStorageSettings(string alias, [FromBody] AddressStorageSettings addressStorageSettings)
        {
            if (addressStorageSettings == null)
                return BadRequest("Body request cannot be null");

            (bool isEdited, string message) = _addressService.EditAddressStorageSettings(alias, addressStorageSettings, "system");
            if (isEdited)
                return Ok(message);

            return BadRequest(message);
        }

        [HttpPut("{alias}/settings/partitions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> PutAddressPartitionSettings(string alias, [FromBody] AddressPartitionSettings addressPartitionSettings)
        {
            if (addressPartitionSettings == null)
                return BadRequest("Body request cannot be null");

            (bool isEdited, string message) = _addressService.EditAddressPartitionSettings(alias, addressPartitionSettings, "system");
            if (isEdited)
                return Ok(message);

            return BadRequest(message);
        }

        [HttpPut("{alias}/settings/replication")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> PutAddressReplicationSettings(string alias, [FromBody] AddressReplicationSettings addressReplicationSettings)
        {
            if (addressReplicationSettings == null)
                return BadRequest("Body request cannot be null");

            (bool isEdited, string message) = _addressService.EditAddressReplicationSettings(alias, addressReplicationSettings, "system");
            if (isEdited)
                return Ok(message);

            return BadRequest(message);
        }

        [HttpPut("{alias}/settings/retention")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> PutAddressRetentionSettings(string alias, [FromBody] AddressRetentionSettings addressRetentionSettings)
        {
            if (addressRetentionSettings == null)
                return BadRequest("Body request cannot be null");

            (bool isEdited, string message) = _addressService.EditAddressRetentionSettings(alias, addressRetentionSettings, "system");
            if (isEdited)
                return Ok(message);

            return BadRequest(message);
        }

        [HttpPut("{alias}/settings/schema")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> PutAddressSchemaSettings(string alias, [FromBody] AddressSchemaSettings addressSchemaSettings)
        {
            if (addressSchemaSettings == null)
                return BadRequest("Body request cannot be null");

            (bool isEdited, string message) = _addressService.EditAddressSchemaSettings(alias, addressSchemaSettings, "system");
            if (isEdited)
                return Ok(message);

            return BadRequest(message);
        }

        [HttpPost("{alias}/promote")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> PromoteAddress(string alias)
        {
            (bool isPromoted, string message) = _addressService.PromoteAddress(alias, "system");
            if (isPromoted)
                return Ok(message);

            return BadRequest(message);
        }

        [HttpDelete("{alias}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> DeleteAddress(string alias)
        {
            (bool isDeleted, string message) = _addressService.DeleteAddress(alias);
            if (isDeleted)
                return Ok(message);

            return BadRequest(message);
        }
    }
}